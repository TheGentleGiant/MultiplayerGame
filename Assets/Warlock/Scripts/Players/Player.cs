using Mirror;

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
    /// Network-synced player data.
    /// </summary>
    [SyncVar] public PlayerData Data;

    public PlayerCast Cast { get; private set; } = null;
    public LifeCycle Life { get; private set; } = null;
    public PlayerMovement Movement { get; private set; } = null;

    private void Awake()
    {
        Cast = GetComponent<PlayerCast>();
        Life = GetComponent<LifeCycle>();
        Movement = GetComponent<PlayerMovement>();
    }

    /// <summary>
    /// Sets <see cref="Local"/> to this instance.
    /// </summary>
    public override void OnStartLocalPlayer() => Local = this;
}