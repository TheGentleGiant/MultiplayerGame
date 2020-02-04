using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ScriptableAbility : ScriptableObject
{
    public string Name => name;
    public Sprite Icon = null;
    public float Cooldown = 1f;
    public float CastTime = 1f;
    public float Range = 1f;

    public virtual bool CanCast(PlayerCast player)
    {
        return !player.IsCasting;
    }

    /// <summary>
    /// Casts the ability, this is where you perform any actions such as spawning objects.
    /// <para>Server-side only.</para>
    /// </summary>
    /// <param name="caster">Casting player.</param>
    /// <param name="position">Target position.</param>
    public abstract void Cast(PlayerCast caster, Vector3 position);

    /// <summary>
    /// Called when the ability begins casting.
    /// <para>Client-side only.</para>
    /// </summary>
    /// <param name="caster">Casting playing.</param>
    public virtual void OnCastBegin(PlayerCast caster, Vector3 position)
    {
    }

    /// <summary>
    /// Called when the ability finishes casting.
    /// <para>Client-side only.</para>
    /// </summary>
    /// <param name="caster">Casting playing.</param>
    public virtual void OnCastEnd(PlayerCast caster, Vector3 position)
    {
    }

    /// <summary>
    /// Caches all abilities in <see cref="Resources"/> on first access attempt.
    /// <see cref="Resources.LoadAll{T}(string)"/> must be called from the main thread, hence why we initialize it on first get.
    /// <para>Key is stable hash-code.</para>
    /// </summary>
    public static Dictionary<int, ScriptableAbility> Cache
    {
        get
        {
            if (cache == null)
            {
                // Load all abilities in resources
                var abilities = Resources.LoadAll<ScriptableAbility>("");

                // Create dictionary from the loaded resources
                // Key is the hash of the name and the value is the ability
                cache = abilities.ToDictionary(x => x.name.GetStableHashCode(), y => y);

                Debug.Log($"Cache: {string.Join(", ", cache.Keys.ToArray())}");
            }

            return cache;
        }
    }
    private static Dictionary<int, ScriptableAbility> cache;
}