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
        AudioMaterial.AudioMaterialType surface = GetSurface();

        GameObject emitter = _footstepEmitter != null ? _footstepEmitter : gameObject;
        AkUnitySoundEngine.SetSwitch("Materials", surface.ToString(), emitter);
        AkUnitySoundEngine.PostEvent(_footstepEvent, emitter);
    }

    private AudioMaterial.AudioMaterialType GetSurface()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        int layerMask = ~excludeLayers;

        if (!Physics.Raycast(origin, Vector3.down, out RaycastHit hit, rayLength, layerMask))
            return AudioMaterial.AudioMaterialType.Grass;

        AudioMaterial audioMaterial = hit.collider.GetComponent<AudioMaterial>()
            ?? hit.collider.GetComponentInParent<AudioMaterial>();
        if (audioMaterial != null)
            return audioMaterial.audioMaterialType;

        Terrain terrain = hit.collider.GetComponent<Terrain>()
            ?? hit.collider.GetComponentInParent<Terrain>();
        if (terrain != null)
        {
            TerrainAudioMaterial terrainAudio = terrain.GetComponent<TerrainAudioMaterial>();
            if (terrainAudio != null)
                return (AudioMaterial.AudioMaterialType)terrainAudio.GetMaterialAtPosition(hit.point);
        }

        return AudioMaterial.AudioMaterialType.Grass;
    }
}
