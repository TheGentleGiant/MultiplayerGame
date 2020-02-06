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
    [Tooltip("Units the radius decays per second.")]
    [SerializeField] private float radiusDecay = 1f;

    [Header("Damage")]
    [Tooltip("Damage done per tick.")]
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
        // We use global values to control the shader
        Shader.SetGlobalVector("_GMaskPosition", transform.position);
        Shader.SetGlobalFloat("_GMaskRadius", radius);
        Shader.SetGlobalFloat("_GMaskFeather", 0f);

        // We're running in edit mode, but this is gameplay related
        if (!Application.isPlaying || !isServer || playerManager == null)
            return;

        // Decrease radius, keeping it within the boundaries
        if (radius > minRadius)
        {
            radius = Mathf.Max(radius - radiusDecay * Time.deltaTime, minRadius);
        }

        UpdateVictims();
        UpdateDamageTick();
    }
    
    /// <summary>
    /// Updates the list of victims, adding/removing them when entering/exiting playable area.
    /// </summary>
    [Server]
    private void UpdateVictims()
    {
        for (var i = 0; i < playerManager.Players.Length; i++)
        {
            var player = playerManager.Players[i];

            // Ignore invalid victims
            if (player == null || player.Life == null || player.Life.IsDead)
                continue;

            // Distance between the player and the center of the sphere
            var distance = Vector3.Distance(player.transform.position, transform.position);

            if (distance > radius)
            {
                // Outside of the radius, should be added to victims
                if (!IsVictim(player))
                    victims.Add((player, NetworkTime.time));
            }
            else
            {
                // Inside radius, remove if in victims
                var index = GetVictim(player);

                if (index >= 0)
                    victims.RemoveAt(index);
            }
        }
    }
    
    /// <summary>
    /// Deals damage to current victims utilizing timestamps for last tick/zone entrance.
    /// </summary>
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

            // If enough time has passed since last tick/zone entrance
            if ((NetworkTime.time - victim.Item2) >= tickInterval)
            {
                // Deal damage and reset timestamp
                victim.Item1.Life.Damage(damage);
                victim.Item2 = NetworkTime.time;

                // These are essentially structs, so must manually update
                victims[i] = victim;
            }
        }
    }

    /// <summary>
    /// Whether the player has been added to our list of victims.
    /// </summary>
    [Server]
    private bool IsVictim(Player player)
    {
        return GetVictim(player) != -1;
    }

    /// <summary>
    /// Returns the index of the player inside of <see cref="victims"/>, or -1 if invalid.
    /// </summary>
    [Server]
    private int GetVictim(Player player)
    {
        for (var i = 0; i < victims.Count; i++)
            if (victims[i].Item1 == player)
                return i;

        return -1;
    }
}