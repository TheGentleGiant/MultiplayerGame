using Mirror;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SphereMask : NetworkBehaviour
{
    [Header("Playable Area")]
    [Tooltip("Current radius of the circle.")]
    [SerializeField, SyncVar] private float radius = 1f;
    [Tooltip("Maximum radius of the circle.")]
    [SerializeField, SyncVar] private float maxRadius = 1f;
    [Tooltip("Minimum radius of the circle.")]
    [SerializeField, SyncVar] private float minRadius = 1f;
    [Tooltip("How fast the radius decays per second.")]
    [SerializeField] private float radiusDecay = 1f;

    [Header("Damage")]
    [Tooltip("How much damage is done per tick.")]
    [SerializeField] private float damage = 2.5f;
    [Tooltip("Second interval between damage ticks.")]
    [SerializeField] private float tickInterval = 1f;

    private PlayerManager playerManager = null;
    private List<(Player, double)> victims = null; // Player/tick timestamp

    public override void OnStartServer()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        victims = new List<(Player, double)>();
        radius = maxRadius;
    }

    private void Update()
    {
        Shader.SetGlobalVector("_GMaskPosition", transform.position);
        Shader.SetGlobalFloat("_GMaskRadius", radius);
        Shader.SetGlobalFloat("_GMaskFeather", 0f); // unused

        // We're running in edit mode as well
        if (!Application.isPlaying || !isServer || playerManager == null)
            return;

        if (radius > minRadius)
        {
            radius = Mathf.Max(radius - radiusDecay * Time.deltaTime, minRadius);
        }

        UpdateVictims();
        UpdateDamageTick();
    }
    
    [Server]
    private void UpdateVictims()
    {
        for (var i = 0; i < playerManager.Players.Length; i++)
        {
            var player = playerManager.Players[i];

            if (player == null || (player.Life != null && player.Life.IsDead))
                continue;

            var distance = Vector3.Distance(player.transform.position, transform.position);

            // Outside of the radius, should be added to victims if not already present
            if (distance > radius)
            {
                if (!IsVictim(player))
                    victims.Add((player, NetworkTime.time));
            }
            // Inside radius, remove if in victims
            else
            {
                var index = GetVictim(player);
                
                if (index >= 0)
                    victims.RemoveAt(index);
            }
        }
    }
    
    [Server]
    private void UpdateDamageTick()
    {
        for (var i = 0; i < victims.Count; i++)
        {
            var victim = victims[i];
            var player = victim.Item1;

            // Remove invalid victims
            if (player == null || player.Life.IsDead)
            {
                victims.RemoveAt(i);
                i--;
                continue;
            }

            // Deal damage and reset timestamp
            if ((NetworkTime.time - victim.Item2) >= tickInterval)
            {
                victim.Item1.Life.Damage(damage);
                victim.Item2 = NetworkTime.time;
                victims[i] = victim;
            }
        }
    }

    [Server]
    private bool IsVictim(Player player)
    {
        return GetVictim(player) != -1;
    }

    [Server]
    private int GetVictim(Player player)
    {
        for (var i = 0; i < victims.Count; i++)
            if (victims[i].Item1 == player)
                return i;

        return -1;
    }
}