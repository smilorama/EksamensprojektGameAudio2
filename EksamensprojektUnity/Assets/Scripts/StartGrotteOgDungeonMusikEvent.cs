using UnityEngine;

public class StartGrotteOgDungeonMusikEvent : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        MusicManager.Instance.StartGrotteOgDungeonMusikEvent();
        gameObject.SetActive(false);
    }
}
