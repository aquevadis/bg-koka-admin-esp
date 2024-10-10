using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;

namespace AdminESP;

public partial class AdminESP
{
    
    private void RegisterListeners()
    {

        //register event listeners
        RegisterEventHandler<EventPlayerSpawn>(OnPlayerSpawn, HookMode.Pre);
        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
        RegisterEventHandler<EventRoundStart>(OnRoundStart);
        
    }

    private void DeregisterListeners()
    {

        //deregister event listeners
        DeregisterEventHandler<EventPlayerSpawn>(OnPlayerSpawn, HookMode.Pre);
        DeregisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
        DeregisterEventHandler<EventRoundStart>(OnRoundStart);

    }

    public HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        var player = @event.Userid;

        if (player is null 
        || player.IsValid is not true 
        || player.Connected is not PlayerConnectedState.PlayerConnected) return HookResult.Continue;

        SetPlayerGlowing(player, player.TeamNum);

        AddTimer(2f, () => {

            if (player is null 
            || player.IsValid is not true 
            || player.Connected is not PlayerConnectedState.PlayerConnected) return;

            //force hiding the glowing props no matter what upon spawn(this is mostly ignored on the first spawn of the round)
            toggleAdminESP[player.Slot] = false;
        });

        return HookResult.Continue;
    }

    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {

        //force hiding the glowing props at round start
        foreach (var player in Utilities.GetPlayers().Where(p => p.Connected is PlayerConnectedState.PlayerConnected)) {
            
            if (toggleAdminESP[player.Slot] is true && player.Team is CsTeam.Spectator && Config.SkipSpectatingEsps is true) 
                continue;
            
            toggleAdminESP[player.Slot] = false;
        }

        return HookResult.Continue;
    }

    public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        var player = @event.Userid;

        if (player is null 
        || player.IsValid is not true 
        || player.Connected is not PlayerConnectedState.PlayerConnected) return HookResult.Continue;

        //remove glowing prop if player has one upon death
        if (glowingPlayers.ContainsKey(player.Slot) is true) {

            if (glowingPlayers[player.Slot].Item1 is not null && glowingPlayers[player.Slot].Item1.IsValid is true
            && glowingPlayers[player.Slot].Item2 is not null && glowingPlayers[player.Slot].Item2.IsValid is true) {
                
                //remove previous modelRelay prop
                glowingPlayers[player.Slot].Item1.AcceptInput("Kill");
                //remove previous modelGlow prop
                glowingPlayers[player.Slot].Item2.AcceptInput("Kill");

            }

            //remove player from the list
            glowingPlayers.Remove(player.Slot);
        }

        return HookResult.Continue;
    }


}