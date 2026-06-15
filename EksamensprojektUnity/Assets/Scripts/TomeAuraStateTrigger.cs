using UnityEngine;

public class TomeAuraStateTrigger : MonoBehaviour
{
    [SerializeField] private string tomeAuraStateGroup;
    [SerializeField] private string tomeAuraSpiritualRoomStateValue;

    private void OnTriggerEnter(Collider other)
    {
        if (MusicGameProgressManager.TomePickedUp)
            return;
        
        if (!other.CompareTag("Player"))
            return;

        AkUnitySoundEngine.SetState(tomeAuraStateGroup, tomeAuraSpiritualRoomStateValue);
    }
}
