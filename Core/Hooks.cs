using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;

namespace AdminESP;

public partial class AdminESP 
{

    public static MemoryFunctionVoid<nint, nint, int, nint, int, short, int, bool>? CheckTransmit;
    public static MemoryFunctionVoid<CCSPlayerPawnBase>? CCSPlayerPawnBase_PostThinkFunc;

    public void LoadHooks() {

        CheckTransmit = new(GameData.GetSignature("CheckTransmit"));

        CheckTransmit.Hook(Hook_CheckTransmit, HookMode.Post);
    }

    public void UnloadHooks() {

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

            //itereate cached players
            for (int j = 0; j < cachedPlayers.Count(); j++) {
                
                //leave self's observerPawn so it can spectate and check if feature is enabled
                //we are clearing the whole spectator list as it doesn't work relaibly per person basis
                if (Config.HideAdminSpectators is true) {

                    if (cachedPlayers[j] is null || cachedPlayers[j].IsValid is not true) continue;

                    //check if it 'us' in the current context and do the magic only if it's not
                    if (cachedPlayers[j].Slot != slot) {

                        //get the target's pawn
                        var targetPawn = cachedPlayers[j].PlayerPawn.Value;
                        if (targetPawn is null || targetPawn.IsValid is not true) continue;

                        //get the target's observerpawn 
                        var targetObserverPawn = cachedPlayers[j].ObserverPawn.Value;
                        if (targetObserverPawn is null 
                        || targetObserverPawn.IsValid is not true) continue;

                        //we clear the spec list via clearing all of the observerTarget' pawns indexes 
                        //from the Observer_services class that any cheat uses as a method to campare 
                        //against current players in the server
                        info.m_pTransmitEntity.Clear((int)targetObserverPawn.Index);
                    }
                }

                //check if admin has enabled ESP 
                if (toggleAdminESP[player.Slot] == true)
                    continue;
                    
                //stop transmitting any entity from the glowingPlayers list
                foreach (var glowingProp in glowingPlayers)
                {

                    if (glowingProp.Value.Item1 is not null && glowingProp.Value.Item1.IsValid is true
                    && glowingProp.Value.Item2 is not null && glowingProp.Value.Item2.IsValid is true) {

                        //prop one
                        info.m_pTransmitEntity.Clear((int)glowingProp.Value.Item1.Index);
                        //prop two
                        info.m_pTransmitEntity.Clear((int)glowingProp.Value.Item2.Index);

                    }

                }

            }

        }
        
        return HookResult.Continue;
    }

}