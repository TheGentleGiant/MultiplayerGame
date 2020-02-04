using Mirror;

public class Player : NetworkBehaviour
{
    public static Player Local { get; private set; } = null;
    public override void OnStartLocalPlayer() => Local = this;

    public PlayerCast Cast { get; private set; } = null;

    private void Awake()
    {
        Cast = GetComponent<PlayerCast>();
    }
}