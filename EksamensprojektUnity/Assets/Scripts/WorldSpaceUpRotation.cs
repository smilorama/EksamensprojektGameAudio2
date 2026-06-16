using UnityEngine;

public class WorldSpaceUpRotation : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
    }
}
