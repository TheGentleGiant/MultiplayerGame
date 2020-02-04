using Mirror;
using UnityEngine;

public class NetManager : NetworkManager
{
    private PlayerManager playerManager = null;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        // Base creates player, so we call the it first
        base.OnServerAddPlayer(conn);

        var player = conn.identity.GetComponent<Player>();

        if (player == null)
        {
            Debug.LogError($"Player prefab doesn't contain a {nameof(Player)} component.");
            return;
        }

        EnsurePlayerManager();
        playerManager.Register(player);
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, NetworkIdentity identity)
    {
        EnsurePlayerManager();

        var player = identity.GetComponent<Player>();

        if (player != null)
        {
            playerManager.Unregister(player);
        }
        else Debug.LogError($"Player prefab doesn't contain a {nameof(Player)} component.");

        // Base destroys the player, so we need to do our thing before calling it
        base.OnServerRemovePlayer(conn, identity);
    }

    private void EnsurePlayerManager()
    {
        if (playerManager == null)
        {
            playerManager = FindObjectOfType<PlayerManager>();

            Debug.Assert(playerManager != null, $"Unable to find {nameof(PlayerManager)}.");
        }
    }
}