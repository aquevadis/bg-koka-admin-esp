using System.Runtime.InteropServices;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;

namespace AdminESP;

public sealed partial class AdminESP : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "Admin ESP";
    public override string ModuleAuthor => "AquaVadis";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleDescription => "Plugin uses code borrowed from CS2Fixes / cs2kz-metamod / hl2sdk / unknown cheats";

    public bool[] toggleAdminESP = new bool[64];

    public Config Config { get; set; } = new();

    public static MemoryFunctionVoid<nint, nint, int, nint, int, short, int, bool>? CheckTransmit;

    public override void Load(bool hotReload)
    {

        RegisterListeners();

        CheckTransmit = new(GameData.GetSignature("CheckTransmit"));

        CheckTransmit.Hook(Hook_CheckTransmit, HookMode.Post);

    }

    public override void Unload(bool hotReload)
    {

        DeregisterListeners();

        CheckTransmit?.Unhook(Hook_CheckTransmit, HookMode.Post);
    }

    private unsafe HookResult Hook_CheckTransmit(DynamicHook hook)
    {

        nint* ppInfoList = (nint*)hook.GetParam<nint>(1);
        int infoCount = hook.GetParam<int>(2);
        
        for (int i = 0; i < infoCount; i++)
        {
            nint pInfo = ppInfoList[i];
            byte slot = *(byte*)(pInfo + GameData.GetOffset("CheckTransmitPlayerSlot"));

            var player = Utilities.GetPlayerFromSlot(slot);
            var info = Marshal.PtrToStructure<CCheckTransmitInfo>(pInfo);

            if (player is null || player.IsValid is not true) continue;

            foreach (var target in Utilities.GetPlayers())
            {

                //skip admins that are in the spectator team only
                if (toggleAdminESP[player.Slot] == true) 
                    continue;

                //stop transmitting any entity from the glowingPlayer list
                foreach (var glowingProp in glowingPlayers)
                {
                    //prop one
                    info.m_pTransmitEntity.Clear((int)glowingProp.Value.Item1.Index);
                    //prop two
                    info.m_pTransmitEntity.Clear((int)glowingProp.Value.Item2.Index);
                }

            }
        }
        
        return HookResult.Continue;
    }

    [ConsoleCommand("css_esp", "")]
    [CommandHelper(minArgs: 0, usage: "", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    [RequiresPermissions("@css/ban")]
    public void OnToggleAdminEsp(CCSPlayerController? player, CommandInfo command)
    {

        if (player is null || player.IsValid is not true) 
            return;

        if (player.Team is not CsTeam.Spectator) {

            SendMessageToSpecificChat(player, msg: "Само в {RED}spectate mode {DEFAULT}можеш да използваш ESP!", print: PrintTo.Chat);
            return;
        }

        toggleAdminESP[player.Slot] = !toggleAdminESP[player.Slot];

    }

    public void OnConfigParsed(Config config)
    {
        Config = config;
    }

}
