using Mirror;
using UnityEngine;

public class ProjectileController : NetworkBehaviour
{
    /// <summary>
    /// Sets the owner of the projectile.
    /// </summary>
    public GameObject Owner { get; set; }

    [Tooltip("How many seconds the projectile stays alive.")]
    [SerializeField] private float lifeTime = 5f;
    [Tooltip("Speed of the projectile.")]
    [SerializeField] private float speed = 5f;
    [Tooltip("Damage done by the projectile.")]
    [SerializeField] private float damage = 10f;

    private Rigidbody body;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();

        if (lifeTime > 0f)
        {
            Invoke(nameof(DestroyNetwork), lifeTime);
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * speed * Time.fixedDeltaTime);
    }
    
    /// <summary>
    /// Handles collision exclusively on the server.
    /// </summary>
    [ServerCallback]
    private void OnTriggerEnter(Collider collider)
    {
        if (Owner == collider.gameObject)
            return;

        var lifeCycle = collider.gameObject.GetComponent<LifeCycle>();

        if (lifeCycle != null)
        {
            lifeCycle.Damage(damage);
        }

        NetworkServer.Destroy(gameObject);
    }

    /// <summary>
    /// Destroys the projectile on all clients. 
    /// </summary>
    [Server]
    private void DestroyNetwork() => NetworkServer.Destroy(gameObject);
}