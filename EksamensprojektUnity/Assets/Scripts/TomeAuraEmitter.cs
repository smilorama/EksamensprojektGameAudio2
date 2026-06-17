using UnityEngine;

public class TomeAuraEmitter : MonoBehaviour
{
    [SerializeField] private string _audioEvent = "Play_4_TomeAura_3D_Loop";

    private void Start()
    {
        AkUnitySoundEngine.PostEvent(_audioEvent, gameObject);
    }
}
