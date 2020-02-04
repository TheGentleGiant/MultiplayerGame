using Mirror;
using System.Collections;
using UnityEngine;

/// <summary>
/// Allows the player to cast abilities.
/// </summary>
public class PlayerCast : NetworkBehaviour
{
    /// <summary>
    /// Templates from which abilities are loaded.
    /// </summary>
    public ScriptableAbility[] Templates => templates;

    /// <summary>
    /// Network-synced list of abilities.
    /// </summary>
    public SyncListAbility Abilities { get; } = new SyncListAbility();

    /// <summary>
    /// Whether the player is currently casting an ability.
    /// </summary>
    public bool IsCasting => activeAbility >= 0;

    /// <summary>
    /// Position from which projectiles are spawned. 
    /// Defaults to transform.position if castTransform isn't set.
    /// </summary>
    public Vector3 CastPosition => castTransform == null ? transform.position : castTransform.position;

    [Tooltip("Transform from which ability projectiles will spawn.")]
    [SerializeField] private Transform castTransform = null;
    [Tooltip("Ability templates, all abilities must be located in Resources.")]
    [SerializeField] private ScriptableAbility[] templates = null;

    [SyncVar(hook = "Hook_ActiveAbility")] private int activeAbility = -1;
    private Animator animator = null;
    private LifeCycle life = null;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        life = GetComponent<LifeCycle>();
    }

    private void Update()
    {
        if (!hasAuthority || IsCasting || (life != null && life.IsDead))
            return;

        if (Input.GetButtonDown("Primary") && Abilities.Count > 0)
            Client_TryCast(0, Client_GetAimPosition());
        else if (Input.GetButtonDown("Secondary") && Abilities.Count > 1)
            Client_TryCast(1, Client_GetAimPosition());
    }

    public override void OnStartServer()
    {
        foreach (var template in templates)
            Abilities.Add(new Ability(template));
    }

    [Client]
    private Vector3 Client_GetAimPosition()
    {
        // Create a plane at cast position facing upwards
        var plane = new Plane(Vector3.up, CastPosition);
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Raycast from the plane
        if (plane.Raycast(ray, out float enter))
        {
            // Get the intersection point
            return ray.GetPoint(enter);
        }

        // Shouldn't be able to fail, but just incase
        return Vector3.zero;
    }

    [Client]
    private void Client_TryCast(int abilityIndex, Vector3 position)
    {
        // Invalid index
        if (abilityIndex < 0 || abilityIndex >= Abilities.Count)
            return;

        var ability = Abilities[abilityIndex];

        // Can't cast
        if (!ability.CanCast(this))
            return;

        // Ask server to cast, server has authority
        Cmd_Cast(abilityIndex, position);
    }

    [Command]
    private void Cmd_Cast(int abilityIndex, Vector3 position)
    {
        var ability = Abilities[abilityIndex];

        // Even if the client thinks it can cast
        // it might not be able to
        if (!ability.CanCast(this))
            return;

        // Set active ability, this is synced
        activeAbility = abilityIndex;

        // Begin casting!
        Server_CastBegin(ability, position);
    }

    [Server]
    private void Server_CastBegin(Ability ability, Vector3 position)
    {
        // Set end of cast time to now + cast time
        ability.CastTimeEnd = NetworkTime.time + ability.CastTime;

        // We've updated a struct, replace old one
        Abilities[activeAbility] = ability;

        // Perform the actual casting
        ability.Cast(this, position);

        // Call client-side effects
        Rpc_CastBegin(activeAbility, position);

        // TODO: Find a better way to do this
        StartCoroutine(CastCoroutine(ability, position));
    }

    [Server]
    private void Server_CastEnd(Ability ability, Vector3 position)
    {
        // Set end of cast time to now + cast time
        ability.CooldownEnd = NetworkTime.time + ability.Cooldown;

        // We've updated a struct, replace old one
        Abilities[activeAbility] = ability;

        // Call client-side effects
        Rpc_CastEnd(activeAbility, position);

        // Invalidate ability index
        activeAbility = -1;
    }

    [ClientRpc]
    private void Rpc_CastBegin(int abilityIndex, Vector3 position)
    {
        Abilities[abilityIndex].Data.OnCastBegin(this, position);

        // If we're have authority of the casting actor
        if (hasAuthority)
        {
            // Get direction to target position, we don't want the height though
            var direction = Vector3.Normalize(position - transform.position);
            direction.y = 0f;

            // Rotate towards target position, the server can't do this for us
            // and since it's just a visual cue, it's fine if it isn't secure
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    [ClientRpc]
    private void Rpc_CastEnd(int abilityIndex, Vector3 position)
    {
        Abilities[abilityIndex].Data.OnCastEnd(this, position);
    }

    [Client]
    public void Hook_ActiveAbility(int newValue)
    {
        // Invalid value or no animator, all this does is set animation
        if (newValue == -1 || animator == null)
            return;
        
        animator.SetTrigger("Cast");
    }

    // HACK: Fast & stupid
    private IEnumerator CastCoroutine(Ability ability, Vector3 position)
    {
        while (ability.IsCasting)
            yield return new WaitForEndOfFrame();

        Server_CastEnd(ability, position);
    }
}