using Mirror;
using UnityEngine;

public class PlayerSimple : NetworkBehaviour
{
    [SerializeField] private GameObject _projectilePrefab = null;
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
    }

    void Start()
    {
        //StartCoroutine(MoveAround());
    }

    /*
    private Vector3 ReturnRandomPos()
    {
        return transform.position += new Vector3(Random.Range(-4f, 4f), Random.Range(-4f, 4f), 0f);
    }
    public IEnumerator MoveAround()
    {
        WaitForSeconds wait = new WaitForSeconds(1f);
        Vector3 startPosition = transform.position;

        while (true)
        {
            transform.position = startPosition + ReturnRandomPos();
            yield return wait;
        }
    }
    */

    private void FixedUpdate()
    {
        if (Input.GetKey(key: KeyCode.W))
        {
            transform.Translate(0, 0, 1f);
        }

        if (Input.GetKey(key: KeyCode.S))
        {
            transform.Translate(0, 0, -1f);
        }

        if (Input.GetKey(key: KeyCode.D))
        {
            transform.Translate(1f, 0, 0);
        }

        if (Input.GetKey(key: KeyCode.A))
        {
            transform.Translate(-1, 0, 0);
        }

        if (Input.GetMouseButtonDown(0) && hasAuthority)
        {
            Cmd_InstantiateNewFireBall();
        }
    }

    [Command]
    private void Cmd_InstantiateNewFireBall()
    {
        GameObject projectile = Instantiate(_projectilePrefab, transform.position,
            Quaternion.identity);

        var controller = projectile.GetComponent<ProjectileController>();
     
        if (controller != null)
        {
            controller.Owner = gameObject;
        }

        NetworkServer.Spawn(projectile);
    }
}