using UnityEngine;
using UnityEditor;

public class FindDegenerateTriangles : EditorWindow
{
    [MenuItem("Wwise/Find Degenerate Geometry Triangles")]
    static void FindDegenerate()
    {
        int totalFound = 0;
        int totalChecked = 0;

        // Check AkSurfaceReflector meshes
        foreach (var reflector in FindObjectsByType<AkSurfaceReflector>(FindObjectsSortMode.None))
        {
            Mesh mesh = reflector.Mesh;
            if (mesh == null)
            {
                if (reflector.TryGetComponent<MeshFilter>(out var filter))
                    mesh = filter.sharedMesh;
            }
            CheckMesh(mesh, reflector.gameObject, "AkSurfaceReflector", ref totalFound, ref totalChecked);
        }

        // Check AkRoom — uses its Collider, which Wwise converts to geometry internally
        foreach (var room in FindObjectsByType<AkRoom>(FindObjectsSortMode.None))
        {
            Mesh mesh = null;
            if (room.TryGetComponent<MeshCollider>(out var mc)) mesh = mc.sharedMesh;
            if (mesh != null)
                CheckMesh(mesh, room.gameObject, "AkRoom MeshCollider", ref totalFound, ref totalChecked);
            else
                Debug.Log($"[GeometryCheck] AkRoom '{room.gameObject.name}': no MeshCollider (primitive collider — Wwise generates geometry)");
        }

        // Check AkRoomPortal — also sends geometry
        foreach (var portal in FindObjectsByType<AkRoomPortal>(FindObjectsSortMode.None))
        {
            Mesh mesh = null;
            var mc = portal.GetComponent<MeshCollider>();
            if (mc != null) mesh = mc.sharedMesh;
            if (mesh != null)
                CheckMesh(mesh, portal.gameObject, "AkRoomPortal MeshCollider", ref totalFound, ref totalChecked);
            else
                Debug.Log($"[GeometryCheck] AkRoomPortal '{portal.gameObject.name}': no MeshCollider");
        }

        Debug.Log($"[GeometryCheck] Done. Checked {totalChecked} mesh(es). Total degenerate triangles: {totalFound}");
    }

    // Replicates Wwise's GetGeometryDataFromMesh vertex deduplication with epsilon-rounding,
    // then checks for degenerate triangles by index — same as what Wwise does internally.
    static void CheckMesh(Mesh mesh, GameObject go, string source, ref int totalFound, ref int totalChecked)
    {
        if (mesh == null)
        {
            Debug.Log($"[GeometryCheck] {source} on '{go.name}': no mesh");
            return;
        }

        totalChecked++;
        Vector3[] rawVerts = mesh.vertices;
        float eps = Vector3.kEpsilon;

        // Build vertRemap using same epsilon-rounding as Wwise
        int[] vertRemap = new int[rawVerts.Length];
        var vertDict = new System.Collections.Generic.Dictionary<Vector3, int>();
        int uniqueCount = 0;

        for (int v = 0; v < rawVerts.Length; v++)
        {
            Vector3 vert = rawVerts[v];
            if (Mathf.Abs(vert.x) < float.MaxValue * eps) vert.x = Mathf.Round(vert.x / eps) * eps;
            if (Mathf.Abs(vert.y) < float.MaxValue * eps) vert.y = Mathf.Round(vert.y / eps) * eps;
            if (Mathf.Abs(vert.z) < float.MaxValue * eps) vert.z = Mathf.Round(vert.z / eps) * eps;

            if (!vertDict.TryGetValue(vert, out int idx))
            {
                idx = uniqueCount++;
                vertDict[vert] = idx;
            }
            vertRemap[v] = idx;
        }

        int degenCount = 0;
        int triIdx = 0;

        for (int s = 0; s < mesh.subMeshCount; s++)
        {
            int[] tris = mesh.GetTriangles(s);
            for (int i = 0; i < tris.Length; i += 3)
            {
                int p0 = vertRemap[tris[i]];
                int p1 = vertRemap[tris[i + 1]];
                int p2 = vertRemap[tris[i + 2]];

                if (p0 == p1 || p0 == p2 || p1 == p2)
                {
                    Debug.LogWarning(
                        $"[GeometryCheck] DEGENERATE tri #{triIdx} — {source} on '{go.name}' (mesh: {mesh.name}, submesh {s}, local tri {i/3})\n" +
                        $"  remapped indices: {p0}, {p1}, {p2}  (original verts: {tris[i]}, {tris[i+1]}, {tris[i+2]})",
                        go);
                    degenCount++;
                    totalFound++;
                }
                triIdx++;
            }
        }

        if (degenCount == 0)
            Debug.Log($"[GeometryCheck] {source} '{go.name}' (mesh: {mesh.name}): {triIdx} tris, {uniqueCount} unique verts — OK");
        else
            Debug.LogWarning($"[GeometryCheck] {source} '{go.name}' (mesh: {mesh.name}): {degenCount} degenerate tri(s) after Wwise vertex deduplication");
    }
}
