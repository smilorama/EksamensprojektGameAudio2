using UnityEngine;

// Attach to Enemy root alongside Animator.
// Call Step() via Animation Events on each footstep frame.

public class EnemyFootsteps : MonoBehaviour
{
    [SerializeField] private string _footstepEvent = "Play_Footstep_Enemy";
    [SerializeField] private GameObject _audioEmitter;
    [SerializeField] private float _rayLength = 2f;
    [SerializeField] private LayerMask _excludeLayers;

    private static readonly string[] _materialSwitches = { "Grass", "Stone", "Dirt", "Tile" };

    public void Step()
    {
        int index = GetSurfaceIndex();
        string switchValue = _materialSwitches[index];

        GameObject emitter = _audioEmitter != null ? _audioEmitter : gameObject;
        AkSoundEngine.SetSwitch("Materials", switchValue, emitter);
        AkSoundEngine.PostEvent(_footstepEvent, emitter);
    }

    private int GetSurfaceIndex()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        int layerMask = ~_excludeLayers;

        if (!Physics.Raycast(origin, Vector3.down, out RaycastHit hit, _rayLength, layerMask))
            return 0;

        AudioMaterial audioMaterial = hit.collider.GetComponent<AudioMaterial>()
            ?? hit.collider.GetComponentInParent<AudioMaterial>();
        if (audioMaterial != null)
            return Mathf.Clamp((int)audioMaterial.audioMaterialType, 0, _materialSwitches.Length - 1);

        Terrain terrain = hit.collider.GetComponent<Terrain>()
            ?? hit.collider.GetComponentInParent<Terrain>();
        if (terrain != null)
        {
            TerrainAudioMaterial terrainAudio = terrain.GetComponent<TerrainAudioMaterial>();
            if (terrainAudio != null)
                return Mathf.Clamp((int)terrainAudio.GetMaterialAtPosition(hit.point), 0, _materialSwitches.Length - 1);
        }

        return 0;
    }
}
