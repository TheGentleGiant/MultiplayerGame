using Mirror;
using System.Collections.Generic;

/// <summary>
/// Provides quick references to all player related behaviours.
/// <para>Does not ensure these behaviours exist, can still be null.</para>
/// </summary>
public class Player : NetworkBehaviour
{
    /// <summary>
    /// Reference to the local player, or null if none.
    /// </summary>
    public static Player Local { get; private set; } = null;

    /// <summary>
    /// Static list of all players, quick access, order doesn't matter.
    /// </summary>
    public static List<Player> Players { get; private set; } = new List<Player>();

    /// <summary>
    /// Network-synced player data.
    /// </summary>
    [SyncVar] public PlayerData Data;

    public PlayerCast Cast { get; private set; } = null;
    public LifeCycle Life { get; private set; } = null;
    public PlayerMovement Movement { get; private set; } = null;

    private void Awake()
    {
        // We don't nullcheck these, not all of them have to exist
        Cast = GetComponent<PlayerCast>();
        Life = GetComponent<LifeCycle>();
        Movement = GetComponent<PlayerMovement>();
    }

    public override void OnStartClient()
    {
        // Clients keep track of every player for UI purposes
        // Server has a manager for this since it needs to be properly set up
        if (!Players.Contains(this))
            Players.Add(this);
    }

    public override void OnNetworkDestroy()
    {
        if (Players.Contains(this))
            Players.Remove(this);
    }

    public override void OnStartLocalPlayer() => Local = this;
}