using Mirror;
using System;
using UnityEngine;

[Serializable]
public struct PlayerData
{
    public string Name;
    public Color Color;

    public PlayerData(string name, Color color)
    {
        Name = name;
        Color = color;
    }
}

/// <summary>
/// Keeps track of players and their default values.
/// <para>Server-side only.</para>
/// </summary>
public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private Player[] players = null;
    [SerializeField] private PlayerData[] playerDefaults =
    {
        new PlayerData("Red", Color.red),
        new PlayerData("Blue", Color.blue),
        new PlayerData("Green", Color.green),
        new PlayerData("Yellow", Color.yellow)
    };

    public override void OnStartServer()
    {
        // One slot for each connection possible
        players = new Player[NetworkManager.singleton.maxConnections];

        // Non-destructive, but unwanted
        Debug.Assert(playerDefaults.Length >= players.Length, $"There are not enough default values to support {players.Length} players.");
    }

    [Server]
    public void Register(Player player)
    {
        var slot = GetAvailableSlot();

        if (slot < 0 || slot > players.Length)
        {
            Debug.LogError($"No more slots available.");
            return;
        }

        // Get default values, since there is no lobby or way to set this
        player.Data = GetPlayerDefaults(slot);
        players[slot] = player;
    }

    [Server]
    public void Unregister(Player player)
    {
        var slot = GetPlayerSlot(player);

        if (slot < 0 || slot > players.Length)
        {
            Debug.LogError($"Player was not defined in slots.");
            return;
        }

        // Remove reference, making the slot available again
        players[slot] = null;
    }

    private int GetAvailableSlot()
    {
        for (int i = 0; i < players.Length; i++)
            if (players[i] == null)
                return i;

        return -1;
    }

    private PlayerData GetPlayerDefaults(int slot)
    {
        if (slot >= playerDefaults.Length)
            return new PlayerData("Undefined", Color.white);

        return playerDefaults[slot];
    }

    private int GetPlayerSlot(Player player)
    {
        if (player == null)
            return -1;

        for (int i = 0; i < players.Length; i++)
            if (player == players[i])
                return i;

        return -1;
    }
}