using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RaycastFaces : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    [SerializeField]private GameObject sphere;

    private void Awake()
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

    private void Start()
    {
        StartCoroutine(NextRaycast());
    }

    private IEnumerator NextRaycast()
    {
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        for (int i = 0; i < triangles.Length; i += 3)
        {
            //Debug.Log(vertices.Length);

            // D6 and D10 because they have 2 triangles per face
            if (triangles.Length == 36 || (triangles.Length == 60 && vertices.Length == 40))
            {
                // Get the vertices of the current face
                Vector3 v0 = transform.TransformPoint(vertices[triangles[i]]);
                Vector3 v1 = transform.TransformPoint(vertices[triangles[i + 1]]);
                Vector3 v2 = transform.TransformPoint(vertices[triangles[i + 2]]);
                Vector3 v3 = transform.TransformPoint(vertices[triangles[i + 3]]);
                Vector3 v4 = transform.TransformPoint(vertices[triangles[i + 4]]);
                Vector3 v5 = transform.TransformPoint(vertices[triangles[i + 5]]);

                // Calculate the face normal
                Vector3 faceNormal = Vector3.Cross(v1 - v0, v2 - v0).normalized;
                //Vector3 faceNormal2 = Vector3.Cross(v4 - v3, v5 - v3).normalized;

                // Perform a raycast from the face centroid along the face normal
                Vector3 faceCentroid = (v0 + v1 + v2 + v3 + v4 + v5) / 6f;

                GameObject test = Instantiate(sphere, faceCentroid, Quaternion.identity);
                test.name = ("Side" + ((i / 6f) + 1));

                i += 3;

                Debug.DrawRay(faceCentroid, faceNormal * 20, Color.red, 50f, true);

                yield return wait;
            }
            // D12 because it has 3 triangles per face
            else if (triangles.Length == 108)
            {
                // Get the vertices of the current face
                Vector3 v0 = transform.TransformPoint(vertices[triangles[i]]);
                Vector3 v1 = transform.TransformPoint(vertices[triangles[i + 1]]);
                Vector3 v2 = transform.TransformPoint(vertices[triangles[i + 2]]);
                Vector3 v3 = transform.TransformPoint(vertices[triangles[i + 3]]);
                Vector3 v4 = transform.TransformPoint(vertices[triangles[i + 4]]);
                Vector3 v5 = transform.TransformPoint(vertices[triangles[i + 5]]);
                Vector3 v6 = transform.TransformPoint(vertices[triangles[i + 6]]);
                Vector3 v7 = transform.TransformPoint(vertices[triangles[i + 7]]);
                Vector3 v8 = transform.TransformPoint(vertices[triangles[i + 8]]);

                // Calculate the face normal
                Vector3 faceNormal = Vector3.Cross(v1 - v0, v2 - v0).normalized;

                // Perform a raycast from the face centroid along the face normal
                Vector3 faceCentroid = (v0 + v1 + v2 + v3 + v4 + v5 + v6 + v7 + v8) / 9f;

                GameObject test = Instantiate(sphere, faceCentroid, Quaternion.identity);
                test.name = ("Side" + ((i / 9f) + 1));

                i += 6;

                Debug.DrawRay(faceCentroid, faceNormal * 20, Color.red, 50f, true);

                yield return wait;
            }
            else
            {
                // Get the vertices of the current face
                Vector3 v0 = transform.TransformPoint(vertices[triangles[i]]);
                Vector3 v1 = transform.TransformPoint(vertices[triangles[i + 1]]);
                Vector3 v2 = transform.TransformPoint(vertices[triangles[i + 2]]);

                // Calculate the face normal
                Vector3 faceNormal = Vector3.Cross(v1 - v0, v2 - v0).normalized;

                // Perform a raycast from the face centroid along the face normal
                Vector3 faceCentroid = (v0 + v1 + v2) / 3f;

                GameObject test = Instantiate(sphere, faceCentroid, Quaternion.identity);
                test.name = ("Side" + ((i / 3f) + 1));

                Debug.DrawRay(faceCentroid, faceNormal * 20, Color.red, 50f, true);

                yield return wait;
            }
        }
    }

    /*private void Update()
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
                    //Debug.Log(hit.distance);
                    // Handle the raycast hit here (e.g., print hit information)
                    //Debug.Log("Raycast hit: " + hit.collider.gameObject.name);
                }
            }

            
        }
    }*/
}
