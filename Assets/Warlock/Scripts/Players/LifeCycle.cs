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
    private new Collider collider = null;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider>();
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

        // Other scripts can still override this, so we have to check everywhere
        // if the player is dead before doing stuff
        animator.SetBool("IsDead", true);
        collider.enabled = false;
    }

    /// <summary>
    /// Exits death-state.
    /// </summary>
    [Server]
    public void Revive()
    {
        if (!IsDead)
            return;

        health = maxHealth;
        animator.SetBool("IsDead", false);
        collider.enabled = true;
    }
}