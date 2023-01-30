using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

[System.Serializable]
public class ComputeSphere : MonoBehaviour
{
    [SerializeField, Range(2, 4000)]
    int resolution = 2;

    [SerializeField, HideInInspector]
    MeshFilter[] meshes;

    [SerializeField, Range(0f, 5f)]
    float radius = 1f;

    [SerializeField]
    Material material;

    [SerializeField]
    ComputeShader computeShader;

    [SerializeField]
    bool useOnValidate = false;
    [SerializeField]
    bool saveOnExit = false;

    public void OnValidate()
    {
        if (useOnValidate)
        {
            GenerateSphere();
        }
    }

    // Start is called before the first frame update
    public void GenerateSphere()
    {
        Stopwatch stopwatch = new Stopwatch();
        if (!useOnValidate)
        {
            stopwatch.Start();
        }

        if (meshes == null || meshes.Length==0)
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

            Vector3[] vertices = new Vector3[resolution*resolution];
            int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];

            ComputeBuffer vertexBuffer = new ComputeBuffer(vertices.Length, sizeof(float) * 3);
            ComputeBuffer triangleBuffer = new ComputeBuffer(triangles.Length, sizeof(float) * 3);

            vertexBuffer.SetData(vertices);
            triangleBuffer.SetData(triangles);

            computeShader.SetBuffer(0, "Vertices", vertexBuffer);
            computeShader.SetBuffer(0, "Triangles", triangleBuffer);

            computeShader.SetInt("Resolution", resolution);
            computeShader.SetVector("LocalUp", directions[i]);
            computeShader.SetVector("AxisA", new Vector3(directions[i].y, directions[i].z, directions[i].x));
            computeShader.SetVector("AxisB", Vector3.Cross(directions[i], new Vector3(directions[i].y, directions[i].z, directions[i].x)));
            computeShader.SetFloat("Radius", radius);

            computeShader.Dispatch(0, (int)Mathf.Ceil(resolution *resolution / 256f), 1, 1);

            vertexBuffer.GetData(vertices);
            triangleBuffer.GetData(triangles);

            triangleBuffer.Dispose();
            vertexBuffer.Dispose();

            meshes[i].sharedMesh.Clear();
            meshes[i].sharedMesh.vertices = vertices;
            meshes[i].sharedMesh.triangles = triangles;
            meshes[i].sharedMesh.RecalculateNormals();

        }

        if (!useOnValidate && saveOnExit)
        {
            Stats stats = gameObject.AddComponent<Stats>();
            stopwatch.Stop();
            stats.SetElapsedTime(stopwatch.Elapsed.ToString("mm\\:ss\\.ff"));
        }
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 30), "Sphere CS"))
            GenerateSphere();
    }
}
