using UnityEngine;

public class TomePossession : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!MusicGameProgressManager.TomePickedUp) return;
        if (!other.CompareTag("Player")) return;

        AkUnitySoundEngine.SetState("EndGameMusicStates", "TomePosession");
    }
}