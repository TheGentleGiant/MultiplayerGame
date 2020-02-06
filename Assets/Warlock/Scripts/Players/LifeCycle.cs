using Mirror;
using UnityEngine;

/// <summary>
/// Handles all things life & death.
/// </summary>
public class LifeCycle : NetworkBehaviour
{
    public bool IsDead => health <= 0f;
    public float Health => health;
    public float MaxHealth => maxHealth;

    [Tooltip("Maximum amount of health.")]
    [SerializeField, SyncVar] private float maxHealth = 100f;
    [SyncVar] private float health = 1f;

    private Animator animator = null;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public override void OnStartServer()
    {
        health = maxHealth;
    }

    /// <summary>
    /// Damages the life-cycle.
    /// </summary>
    [Server]
    public void Damage(float value)
    {
        if (IsDead || value < 0f)
            return;

        health = Mathf.Max(0f, health - value);

        if (health <= 0f)
            Kill();
    }
    
    /// <summary>
    /// Heals the life-cycle.
    /// </summary>
    [Server]
    public void Heal(float value)
    {
        if (IsDead || value < 0f)
            return;

        health = Mathf.Min(health + value, maxHealth);
    }

    /// <summary>
    /// Called when the object dies from damage.
    /// </summary>
    [Server]
    public void Kill()
    {
        // Ensure health is zero
        health = 0f;

        // TODO: Handle death
        Debug.Log($"{nameof(LifeCycle)}: Player {netId} died.");

        // Let clients know we died
        Rpc_OnDeath();
    }

    /// <summary>
    /// Exits death-state.
    /// </summary>
    [Server]
    public void Revive()
    {
        if (!IsDead)
            return;

        // Reset health
        health = maxHealth;

        // Let clients know we've been revived
        Rpc_OnRevive();
    }

    /// <summary>
    /// Called on clients on death.
    /// </summary>
    [ClientRpc]
    public void Rpc_OnDeath()
    {
        if (animator == null)
            return;

        animator.SetBool("IsDead", true);
    }

    /// <summary>
    /// Called on clients on revive.
    /// </summary>
    [ClientRpc]
    public void Rpc_OnRevive()
    {
        if (animator == null)
            return;

        animator.SetBool("IsDead", false);
    }
}