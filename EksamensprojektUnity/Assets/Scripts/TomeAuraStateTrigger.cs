using UnityEngine;

public class TomeAuraStateTrigger : MonoBehaviour
{
    [SerializeField] private string tomeAuraStateGroup;
    [SerializeField] private string tomeAuraStateValue;

    private void OnTriggerEnter(Collider other)
    {
        if (MusicGameProgressManager.TomePickedUp)
            return;
        
        if (!other.CompareTag("Player"))
            return;

        AkUnitySoundEngine.SetState(tomeAuraStateGroup, tomeAuraStateValue);
    }
}
