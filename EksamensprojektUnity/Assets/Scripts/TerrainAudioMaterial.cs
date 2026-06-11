using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class TerrainAudioMaterial : MonoBehaviour
{
    [Tooltip("Mapping: indeks matcher terrain texture layer. 0=Grass, 1=Stone, 2=Dirt")]
    [SerializeField] private AudioMaterial.AudioMaterialType[] layerToMaterial = new AudioMaterial.AudioMaterialType[]
    {
        AudioMaterial.AudioMaterialType.Grass,
        AudioMaterial.AudioMaterialType.Stone,
        AudioMaterial.AudioMaterialType.Dirt
    };

    [SerializeField] private AudioMaterial.AudioMaterialType fallback = AudioMaterial.AudioMaterialType.Grass;

    private Terrain _terrain;
    private TerrainData _terrainData;

    private void Awake()
    {
        _terrain = GetComponent<Terrain>();
        _terrainData = _terrain != null ? _terrain.terrainData : null;
    }

    public float GetMaterialAtPosition(Vector3 worldPosition)
    {
        if (_terrain == null) _terrain = GetComponent<Terrain>();
        if (_terrainData == null) _terrainData = _terrain != null ? _terrain.terrainData : null;
        if (_terrainData == null) return (float)fallback;

        Vector3 localPos = worldPosition - _terrain.transform.position;
        float nx = Mathf.Clamp01(localPos.x / _terrainData.size.x);
        float nz = Mathf.Clamp01(localPos.z / _terrainData.size.z);

        int mapX = Mathf.Clamp((int)(nx * _terrainData.alphamapWidth),  0, _terrainData.alphamapWidth  - 1);
        int mapZ = Mathf.Clamp((int)(nz * _terrainData.alphamapHeight), 0, _terrainData.alphamapHeight - 1);

        float[,,] alphaMaps = _terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

        int dominantLayer = 0;
        float maxWeight = 0f;
        for (int i = 0; i < _terrainData.alphamapLayers; i++)
        {
            if (alphaMaps[0, 0, i] > maxWeight)
            {
                maxWeight = alphaMaps[0, 0, i];
                dominantLayer = i;
            }
        }

        if (dominantLayer < layerToMaterial.Length)
            return (float)layerToMaterial[dominantLayer];
        return (float)fallback;
    }
}
