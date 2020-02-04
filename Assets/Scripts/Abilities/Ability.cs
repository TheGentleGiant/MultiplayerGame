using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// Network-synced ability list.
/// </summary>
public class SyncListAbility : SyncList<Ability>
{
}

[Serializable]
public struct Ability
{
    /// <summary>
    /// Gets the associated <see cref="ScriptableAbility"/> from cache.
    /// <para>Uses <see cref="NetworkHash"/> to get value.</para>
    /// </summary>
    public ScriptableAbility Data
    {
        get
        {
            if (!ScriptableAbility.Cache.ContainsKey(NetworkHash))
                throw new KeyNotFoundException($"Unable to find hash {NetworkHash} in ability cache.\n" +
                    $"Ensure abilities are inside the Resources folder.");

            return ScriptableAbility.Cache[NetworkHash];
        }
    }

    /// <summary>
    /// Network Hash calculated from the name of the <see cref="ScriptableAbility"/>.
    /// <para>Used to reference the abiltiy cross-network.</para>
    /// </summary>
    public int NetworkHash;
    public double CastTimeEnd;
    public double CooldownEnd;

    public string Name => Data.Name;
    public Sprite Icon => Data.Icon;
    public float Cooldown => Data.Cooldown;
    public float CastTime => Data.CastTime;
    public float Range => Data.Range;
    public bool IsCasting => CastTimeLeft > 0f;
    public bool IsCooldown => CooldownLeft > 0f;

    /// <summary>
    /// Seconds until the cooldown ends.
    /// </summary>
    public float CooldownLeft => Mathf.Max(0f, (float)(CooldownEnd - NetworkTime.time));

    /// <summary>
    /// Seconds until the cast time ends.
    /// </summary>
    public float CastTimeLeft => Mathf.Max(0f, (float)(CastTimeEnd - NetworkTime.time));

    public Ability(ScriptableAbility data)
    {
        NetworkHash = data.Name.GetStableHashCode();
        CooldownEnd = CastTimeEnd = NetworkTime.time;
    }

    public bool CanCast(PlayerCast caster)
    {
        return Data.CanCast(caster) && !IsCooldown && !IsCasting;
    }

    public void Cast(PlayerCast caster, Vector3 position)
    {
        Data.Cast(caster, position);
    }
}