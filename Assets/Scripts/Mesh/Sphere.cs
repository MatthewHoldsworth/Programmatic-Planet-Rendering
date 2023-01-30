using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

[System.Serializable]
public class Sphere : MonoBehaviour
{
    [SerializeField, Range(2, 4000)]
    int resolution = 2;

    [SerializeField, Range(0.1f, 10f)]
    float radius = 1f;

    [SerializeField]
    bool useNoise = false;
    [SerializeField]
    bool useOnValidate = false;
    [SerializeField]
    bool saveOnExit = false;

    [SerializeField]
    NoiseFilter noiseFilter;

    [SerializeField]
    Material material;

    [SerializeField,HideInInspector]
    MeshFilter[] meshes;

    // Start is called before the first frame update
    public void OnValidate()
    {
        if (useOnValidate)
        {
            GenerateSphere();
        }
    }

    public void GenerateSphere()
    {
        Stopwatch stopwatch = new Stopwatch();
        if (!useOnValidate)
        {
            stopwatch.Start();
        }

        if (meshes == null || meshes.Length == 0)
        {
            meshes = new MeshFilter[6];
        }

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.forward, Vector3.right, Vector3.back, Vector3.left };

        for (int i = 0; i < directions.Length; i++)
        {
            if (meshes[i] == null)
            {
                GameObject face = new GameObject("Face" + i, typeof(MeshFilter), typeof(MeshRenderer), typeof(LODGroup));

                face.GetComponent<MeshRenderer>().sharedMaterial = material;
                face.transform.SetParent(gameObject.transform);

                Renderer[] renderers = new Renderer[1];
                renderers[0] = face.GetComponent<Renderer>();
                LOD[] lods = new LOD[1];
                lods[0] = new LOD(0.1f, renderers);

                face.GetComponent<LODGroup>().SetLODs(lods);
                face.GetComponent<LODGroup>().RecalculateBounds();

                meshes[i] = face.GetComponent<MeshFilter>();
                meshes[i].sharedMesh = new Mesh();
                meshes[i].sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            }
            Vector3[] vertices = new Vector3[resolution * resolution];
            int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];

            GeneratePlane(directions[i], i, meshes[i]);
        }

        if (!useOnValidate && saveOnExit)
        {
            Stats stats = gameObject.AddComponent<Stats>();
            stopwatch.Stop();
            stats.SetElapsedTime(stopwatch.Elapsed.ToString("mm\\:ss\\.ff"));
        }
    }

    // Update is called once per frame
    void GeneratePlane(Vector3 localUp, int index, MeshFilter mesh)
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        //Perpendicular to localUp
        Vector3 axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        //Normal of axisA and localUp
        Vector3 axisB = Vector3.Cross(localUp, axisA);
        //Both used as the x and y of the current position on the plane

        int triIndex = 0;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + (y * resolution);
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                switch (useNoise) {
                    case true:
                        vertices[i] = pointOnUnitSphere * noiseFilter.Elevate(pointOnUnitSphere) * radius;
                        break;
                    case false:
                        vertices[i] = pointOnUnitSphere * radius;
                        break;
                        }

                if (x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;
                    triIndex += 6;
                }
            }
        }

        mesh.sharedMesh.Clear();
        mesh.sharedMesh.vertices = vertices;
        mesh.sharedMesh.triangles = triangles;
        mesh.sharedMesh.RecalculateNormals();
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 30), "Sphere C#"))
            GenerateSphere();
    }
}
