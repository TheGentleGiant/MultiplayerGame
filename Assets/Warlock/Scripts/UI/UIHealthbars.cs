using UnityEngine;

public class UIHealthbars : MonoBehaviour
{
    [SerializeField] private UIHealthbar healthbarPrefab = null;
    [SerializeField] private Vector3 offset = Vector3.up * 4f;

    private void LateUpdate()
    {
        var players = Player.Players;

        if (players == null || players.Count <= 0)
        {
            // Make sure to remove healthbars if we leave
            ResetInstances(0);
            return;
        }

        // Creates/removes slots to fit number of players
        ResetInstances(players.Count);

        for (var i = 0; i < players.Count; i++)
        {
            var player = players[i];
            var child = transform.GetChild(i);
            var bar = child.GetComponent<UIHealthbar>();
            var position = Camera.main.WorldToScreenPoint(player.transform.position + offset);

            if (bar == null || player.Life == null)
            {
                child.gameObject.SetActive(false);
                continue;
            }
            else if (!child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(true);
            }

            bar.FillImage.fillAmount = (player.Life.Health / player.Life.MaxHealth);
            bar.transform.position = position;
        }
    }

    private void ResetInstances(int numPlayers)
    {
        // Spawn new slots
        for (var i = transform.childCount; i < numPlayers; i++)
        {
            var slot = Instantiate(healthbarPrefab);
            slot.transform.SetParent(transform);
        }

        // Destroy excess slots
        for (var i = transform.childCount - 1; i >= numPlayers; i--)
            Destroy(transform.GetChild(i).gameObject);
    }
}