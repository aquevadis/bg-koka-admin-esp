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
    public override string ModuleVersion => "1.0.5s";
    public override string ModuleDescription => "Plugin uses code borrowed from CS2Fixes / cs2kz-metamod / hl2sdk / unknown cheats and xstage from CS# discord";

    public bool[] toggleAdminESP = new bool[64];
    public Config Config { get; set; } = new();

    public override void Load(bool hotReload)
    {
        LoadHooks();
        RegisterListeners();
        
    }

    public override void Unload(bool hotReload)
    {

        UnloadHooks();
        DeregisterListeners();
    }

    [ConsoleCommand("css_esp", "Toggle Admin ESP")]
    [CommandHelper(minArgs: 0, whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    public void OnToggleAdminEsp(CCSPlayerController? player, CommandInfo command)
    {

        if (player is null || player.IsValid is not true) return;

        if (AdminManager.PlayerHasPermissions(player, Config.AdminFlag) is not true) {

            SendMessageToSpecificChat(player, msg: "Admin ESP can only be used from {GREEN}admins{DEFAULT}!", print: PrintTo.Chat);
            return;  
        }

        string actionIdentifier = command.GetArg(1);

        switch (player.PawnIsAlive) {
            
            case true:

                if (Config.AllowDeadAdminESP is not true) {
                    SendMessageToSpecificChat(player, msg: "Admin ESP while dead is {RED}disabled{DEFAULT}!", print: PrintTo.Chat);
                    return;
                }
                SendMessageToSpecificChat(player, msg: "Admin ESP is only allowed while {RED}dead{DEFAULT}!", print: PrintTo.Chat);

            break;
            case false:

                if (player.Team is CsTeam.Spectator) {
                    toggleAdminESP[player.Slot] = !toggleAdminESP[player.Slot];
                    SendMessageToSpecificChat(player, msg: $"Admin ESP has been " + (toggleAdminESP[player.Slot] ? "{GREEN}enabled!" : "{RED}disabled!"), print: PrintTo.Chat); 
                    return;
                }
                else {
                    if (Config.AllowDeadAdminESP is true) {
                        toggleAdminESP[player.Slot] = !toggleAdminESP[player.Slot];
                        SendMessageToSpecificChat(player, msg: $"Admin ESP has been " + (toggleAdminESP[player.Slot] ? "{GREEN}enabled!" : "{RED}disabled!"), print: PrintTo.Chat);
                        return;
                    }
                }

                SendMessageToSpecificChat(player, msg: "Admin ESP is only allowed in {RED}spectate mode{DEFAULT}!", print: PrintTo.Chat);
            break;
        }

    }
    public void OnConfigParsed(Config config)
    {
        Config = config;
    }

}
