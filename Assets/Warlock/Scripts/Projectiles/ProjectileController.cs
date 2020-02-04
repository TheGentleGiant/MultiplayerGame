using Mirror;
using UnityEngine;

public class ProjectileController : NetworkBehaviour
{
    /// <summary>
    /// Sets the owner of the projectile.
    /// </summary>
    public GameObject Owner { get; set; } = null;

    [Tooltip("How many seconds the projectile stays alive.")]
    [SerializeField] private float lifeTime = 5f;
    [Tooltip("Speed of the projectile.")]
    [SerializeField] private float speed = 5f;
    [Tooltip("Damage done by the projectile.")]
    [SerializeField] private float damage = 10f;

    private Rigidbody body = null;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    public override void OnStartServer()
    {
        if (lifeTime > 0f)
        {
            Invoke(nameof(DestroyNetwork), lifeTime);
        }
    }

    private void FixedUpdate()
    {
        var position = transform.position + (transform.forward * speed * Time.fixedDeltaTime);
        body.MovePosition(position);
    }
    
    /// <summary>
    /// Handles collision exclusively on the server.
    /// </summary>
    [ServerCallback]
    private void OnTriggerEnter(Collider collider)
    {
        if (Owner == collider.gameObject)
            return;

        var lifeCycle = collider.GetComponent<LifeCycle>();

        if (lifeCycle != null)
        {
            if (lifeCycle.IsDead)
                return;

            lifeCycle.Damage(damage);
        }

        var movement = collider.GetComponent<PlayerMovement>();

        if (movement != null)
        {
            movement.Knockback(transform.forward * 10f);
        }

        DestroyNetwork();
    }

    /// <summary>
    /// Destroys the projectile on all clients. 
    /// </summary>
    [Server]
    private void DestroyNetwork() => NetworkServer.Destroy(gameObject);
}