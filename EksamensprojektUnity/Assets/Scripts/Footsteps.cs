using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Footsteps : MonoBehaviour
{
    [SerializeField] private float walkInterval = 0.5f;
    [SerializeField] private float sprintInterval = 0.3f;
    [SerializeField] private float sprintSpeedThreshold = 5f;
    [SerializeField] private float moveThreshold = 0.1f;
    [SerializeField] private float rayLength = 2f;
    [SerializeField] private LayerMask excludeLayers;
    [SerializeField] private string _footstepEvent = "Play_Footstep";
    [SerializeField] private GameObject _footstepEmitter;

    private CharacterController _controller;
    private float _timer;

    private static readonly string[] _materialSwitches = { "Grass", "Stone", "Dirt", "Tile" };

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float speed = new Vector3(_controller.velocity.x, 0f, _controller.velocity.z).magnitude;

        if (!_controller.isGrounded || speed < moveThreshold)
        {
            _timer = 0f;
            return;
        }

        float interval = speed > sprintSpeedThreshold ? sprintInterval : walkInterval;
        _timer += Time.deltaTime;

        if (_timer >= interval)
        {
            _timer = 0f;
            PlayFootstep();
        }
    }

    private void PlayFootstep()
    {
        int surfaceIndex = GetSurfaceIndex();
        string switchValue = _materialSwitches[surfaceIndex];

        GameObject emitter = _footstepEmitter != null ? _footstepEmitter : gameObject;
        AkSoundEngine.SetSwitch("Materials", switchValue, emitter);
        AkSoundEngine.PostEvent(_footstepEvent, emitter);
    }

    private int GetSurfaceIndex()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        int layerMask = ~excludeLayers;

        if (!Physics.Raycast(origin, Vector3.down, out RaycastHit hit, rayLength, layerMask))
            return 0;

        AudioMaterial audioMaterial = hit.collider.GetComponent<AudioMaterial>()
            ?? hit.collider.GetComponentInParent<AudioMaterial>();
        if (audioMaterial != null)
            return (int)audioMaterial.audioMaterialType;

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
