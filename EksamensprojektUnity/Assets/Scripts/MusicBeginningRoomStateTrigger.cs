using UnityEngine;

public class MusicBeginningRoomStateTrigger : MonoBehaviour
{
    [SerializeField] private string stateGroup;
    [SerializeField] private string stateValue;

    private void OnTriggerEnter(Collider other)
    {
        if (MusicGameProgressManager.TomePickedUp)
            return;
        
        if (!other.CompareTag("Player"))
            return;

        AkUnitySoundEngine.SetState(stateGroup, stateValue);
    }
}
