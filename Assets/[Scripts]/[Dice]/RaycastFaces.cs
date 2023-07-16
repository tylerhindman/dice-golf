using UnityEngine;

public class RaycastFaces : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    private void Start()
    {
        // Get the MeshFilter component and its shared mesh
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter component not found!");
            return;
        }

        mesh = meshFilter.sharedMesh;
        if (mesh == null)
        {
            Debug.LogError("Mesh not found!");
            return;
        }

        // Extract vertices and triangles from the mesh
        vertices = mesh.vertices;
        triangles = mesh.triangles;

        // Perform a raycast from each face
    }

    private void Update()
    {
        for (int i = 0; i < triangles.Length; i += 3)
        {
            // Get the vertices of the current face
            Vector3 v0 = transform.TransformPoint(vertices[triangles[i]]);
            Vector3 v1 = transform.TransformPoint(vertices[triangles[i + 1]]);
            Vector3 v2 = transform.TransformPoint(vertices[triangles[i + 2]]);

            // Calculate the face normal
            Vector3 faceNormal = Vector3.Cross(v1 - v0, v2 - v0).normalized;

            // Perform a raycast from the face centroid along the face normal
            Vector3 faceCentroid = (v0 + v1 + v2) / 3f;
            RaycastHit hit;
            Debug.DrawRay(faceCentroid, faceNormal * 20, Color.red);
            if (Physics.Raycast(faceCentroid, faceNormal * 20, out hit))
            {
                if (hit.collider.tag == "Ground")
                {
                    Debug.Log(hit.distance);
                    // Handle the raycast hit here (e.g., print hit information)
                    Debug.Log("Raycast hit: " + hit.collider.gameObject.name);
                }
            }
        }
    }
}
