using Mirror;
using UnityEngine;

public class NetManager : NetworkManager
{
    private PlayerManager playerManager = null;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        // Base creates the player, so we call it first
        base.OnServerAddPlayer(conn);

        // Get the player component, which contains player data
        var player = conn.identity.GetComponent<Player>();

        // Ensure it exists, otherwise we done goofed
        if (player == null)
        {
            Debug.LogError($"Player prefab doesn't contain a {nameof(Player)} component.");
            return;
        }

        // We need the player manager, can't get it on Awake nor OnServerStart
        // so we look for it when we need it
        if (FindPlayerManager())
            playerManager.Register(player);
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, NetworkIdentity identity)
    {
        var player = identity.GetComponent<Player>();

        if (player != null)
        {
            if (FindPlayerManager())
                playerManager.Unregister(player);
        }
        else Debug.LogError($"Player prefab doesn't contain a {nameof(Player)} component.");

        // Base destroys the player, so we need to do our thing before calling it
        base.OnServerRemovePlayer(conn, identity);
    }

    private bool FindPlayerManager()
    {
        if (playerManager == null)
        {
            playerManager = FindObjectOfType<PlayerManager>();

            Debug.Assert(playerManager != null, $"Unable to find {nameof(PlayerManager)}.");
            return playerManager != null;
        }

        return true;
    }
}