using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

namespace AdminESP;

public partial class AdminESP 
{

    public static MemoryFunctionVoid<nint, nint, int, nint, int, short, int, bool>? CheckTransmit;

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

            foreach (var target in Utilities.GetPlayers())
            {

                //check if admin has enabled ESP 
                if (toggleAdminESP[player.Slot] == true) {

                    //leave self's observerPawn so it can spectate
                    if (target.Slot != slot && Config.HideAdminSpectators is true) {

                        //get the target's pawn
                        var targetPawn = target.PlayerPawn.Value;
                        if (targetPawn is null || targetPawn.IsValid is not true) continue;

                        //get the target's observerpawn 
                        var targetObserverPawn = target.ObserverPawn.Value;
                        if (targetObserverPawn is null 
                        || targetObserverPawn.IsValid is not true) continue;

                        //we clear the spec list via clearing all of the observerTarget' pawns indexes 
                        //from the Observer_services class that any cheat uses as a method to campare 
                        //against current players in the server
                        info.m_pTransmitEntity.Clear((int)targetObserverPawn.Index);

                    }
                    
                    continue;
                }
                    
                //stop transmitting any entity from the glowingPlayers list
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

}