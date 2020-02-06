using UnityEngine;

public class UIHotbar : MonoBehaviour
{
    [SerializeField] private UIHotbarSlot slotPrefab = null;

    private void Update()
    {
        var player = Player.Local;

        if (player == null)
        {
            // Remove all instances to get rid of the UI
            ResetInstances(0);
            return;
        }

        var numAbilities = player.Cast.Abilities.Count;

        // Creates/removes slots to fit number of abilities
        ResetInstances(numAbilities);

        for (var i = 0; i < numAbilities; i++)
        {
            var child = transform.GetChild(i);
            var slot = child.GetComponent<UIHotbarSlot>();

            if (slot == null || slot.Icon == null || slot.Cooldown == null)
            {
                // De-activate since it's invalid
                child.gameObject.SetActive(false);
                continue;
            }
            else if (!child.gameObject.activeSelf)
            {
                // If the slot has become valid after being de-activated
                // we need to enable it again
                child.gameObject.SetActive(true);
            }

            var template = player.Cast.Abilities[i];

            slot.Icon.sprite = template.Icon;
            slot.Cooldown.fillAmount = (template.CastTimeLeft > 0f) ? 1f : (template.CooldownLeft / template.Cooldown);
        }
    }

    private void ResetInstances(int numAbilities)
    {
        // Spawn new slots
        for (var i = transform.childCount; i < numAbilities; i++)
        {
            var slot = Instantiate(slotPrefab);
            slot.transform.SetParent(transform);
        }

        // Destroy excess slots
        for (var i = transform.childCount - 1; i >= numAbilities; i--)
            Destroy(transform.GetChild(i).gameObject);
    }
}