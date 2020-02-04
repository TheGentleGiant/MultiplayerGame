﻿using Mirror;

public class Player : NetworkBehaviour
{
    public static Player Local { get; private set; } = null;
    public override void OnStartLocalPlayer() => Local = this;

    [SyncVar] public PlayerData Data;
    public PlayerCast Cast { get; private set; } = null;
    public LifeCycle Life { get; private set; } = null;

    private void Awake()
    {
        Cast = GetComponent<PlayerCast>();
        Life = GetComponent<LifeCycle>();
    }
}