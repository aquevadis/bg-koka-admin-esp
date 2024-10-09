using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
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
    }

    private void DeregisterListeners()
    {

        //deregister event listeners
        DeregisterEventHandler<EventPlayerSpawn>(OnPlayerSpawn, HookMode.Pre);
        DeregisterEventHandler<EventPlayerDeath>(OnPlayerDeath);

    }

    public HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        var player = @event.Userid;

        if (player is null 
        || player.IsValid is not true 
        || player.Connected is not PlayerConnectedState.PlayerConnected) return HookResult.Continue;

        SetPlayerGlowing(player, player.TeamNum);

        //force hiding the glowing props no matter what upon spawn(player is playing)
        toggleAdminESP[player.Slot] = false;

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

            //remove previous modelRelay prop
            glowingPlayers[player.Slot].Item1.AddEntityIOEvent("Kill", glowingPlayers[player.Slot].Item1, null, "", 2f);
            //remove previous modelGlow prop
            glowingPlayers[player.Slot].Item2.AddEntityIOEvent("Kill", glowingPlayers[player.Slot].Item2, null, "", 2f);

            //remove player from the list
            glowingPlayers.Remove(player.Slot);
        }

        return HookResult.Continue;
    }


}