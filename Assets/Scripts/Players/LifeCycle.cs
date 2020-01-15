using Mirror;
using UnityEngine;

public class LifeCycle : NetworkBehaviour
{
    [Tooltip("Whether the object hasn't died yet.")]
    [SerializeField, SyncVar] private bool isAlive = true;
    [Tooltip("Amount of health of the player.")]
    [SerializeField, SyncVar] private float health = 100f;

    /// <summary>
    /// Damages the life cycle.
    /// </summary>
    [Server]
    public void Damage(float damage)
    {
        health = Mathf.Max(0f, health - damage);

        Debug.Log($"{nameof(LifeCycle)}: Player {netId} took {damage} damage ({health} remaining).");

        if (health <= 0f)
            Die();
    }
    
    /// <summary>
    /// Called when the object dies from loss of health.
    /// </summary>
    [Server]
    public void Die()
    {
        isAlive = false;

        Debug.Log($"{nameof(LifeCycle)}: Player {netId} died.");
    }

    /// <summary>
    /// Destroys the object on all clients. 
    /// </summary>
    [Server]
    private void DestroyNetwork() => NetworkServer.Destroy(gameObject);
}