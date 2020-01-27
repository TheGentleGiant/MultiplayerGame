using Mirror;
using UnityEngine;

[CreateAssetMenu(menuName = "Warlock/Abilities/Projectile Ability")]
public class ProjectileAbility : ScriptableAbility
{
    [SerializeField] private ProjectileController projectilePrefab = null;

    public override void Cast(PlayerCast caster, Vector3 position)
    {
        var direction = Vector3.Normalize(position - caster.transform.position);
        var controller = Instantiate(projectilePrefab, caster.transform.position, Quaternion.LookRotation(direction));

        Debug.Log($"cajkcasic");

        NetworkServer.Spawn(controller.gameObject);
    }
}