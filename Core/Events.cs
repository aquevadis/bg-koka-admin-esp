using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;

namespace AdminESP;

public partial class AdminESP
{
    
    private void RegisterListeners()
    {

        RegisterListener<Listeners.OnClientAuthorized>(OnClientAuthorized);
        RegisterListener<Listeners.OnClientConnected>(OnClientConnected);
        RegisterListener<Listeners.OnClientPutInServer>(OnClientPutInServer);
        RegisterListener<Listeners.OnClientDisconnect>(OnClientDisconnected);

        //register event listeners
        RegisterEventHandler<EventPlayerSpawn>(OnPlayerSpawn, HookMode.Pre);
        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
        RegisterEventHandler<EventRoundStart>(OnRoundStart);
        
    }

    private void DeregisterListeners()
    {

        RemoveListener<Listeners.OnClientAuthorized>(OnClientAuthorized);
        RemoveListener<Listeners.OnClientConnected>(OnClientConnected);
        RemoveListener<Listeners.OnClientPutInServer>(OnClientPutInServer);
        RemoveListener<Listeners.OnClientDisconnect>(OnClientDisconnected);

        //deregister event listeners
        DeregisterEventHandler<EventPlayerSpawn>(OnPlayerSpawn, HookMode.Pre);
        DeregisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
        DeregisterEventHandler<EventRoundStart>(OnRoundStart);

    }

    private void OnClientAuthorized(int slot, SteamID steamid)
    {

        var player = Utilities.GetPlayerFromSlot(slot);
        if(player == null || player.IsValid is not true) return;

        if (cachedPlayers.Contains(player) is not true)
            cachedPlayers.Add(player);
        
    }

    private void OnClientConnected(int slot)
    {

        var player = Utilities.GetPlayerFromSlot(slot);
        if(player == null || player.IsValid is not true) return;

        if (cachedPlayers.Contains(player) is not true)
            cachedPlayers.Add(player);
        
    }

    private void OnClientPutInServer(int slot)
    {
        var player = Utilities.GetPlayerFromSlot(slot);
        if (player is null || player.IsBot is not true) return;

        if (cachedPlayers.Contains(player) is not true)
            cachedPlayers.Add(player);
        
    }

    public HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        var player = @event.Userid;

        if (player is null 
        || player.IsValid is not true 
        || player.Connected is not PlayerConnectedState.PlayerConnected) return HookResult.Continue;

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

        for (int i = 0; i < cachedPlayers.Count(); i++) {

            if (cachedPlayers[i] is null || cachedPlayers[i].IsValid is not true) continue;

            if (toggleAdminESP[cachedPlayers[i].Slot] is true && cachedPlayers[i].Team is CsTeam.Spectator && Config.SkipSpectatingEsps is true) 
                continue;
            
            toggleAdminESP[cachedPlayers[i].Slot] = false;

        }

        if (togglePlayersGlowing is true)
            togglePlayersGlowing = false;

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

    private void OnClientDisconnected(int slot)
    {

        var player = Utilities.GetPlayerFromSlot(slot);
        if (player == null || player.IsValid is not true) return;

        if (cachedPlayers.Contains(player) is true)
            cachedPlayers.Remove(player);
        
    }


}