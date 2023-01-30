using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class GradientCS : MonoBehaviour
{

    [SerializeField]
    Gradient gradient;
    [SerializeField]
    ComputeShader computeShader;

    //Mesh mesh;
    // Start is called before the first frame update
    void GenerateRegions()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        Mesh mesh;
        for (int i = 0; i<transform.childCount;i++) {
            if (transform.GetChild(i).GetComponent<MeshFilter>() != null)
            {
                mesh = transform.GetChild(i).GetComponent<MeshFilter>().sharedMesh;


                Color[] colors = new Color[mesh.vertices.Length];

                GradientColorKey[] colourKeys = gradient.colorKeys;

                ComputeBuffer colourBuffer = new ComputeBuffer(mesh.vertices.Length, sizeof(float) * 4);
                ComputeBuffer vertexBuffer = new ComputeBuffer(mesh.vertices.Length, sizeof(float) * 3);
                ComputeBuffer colourKeysBuffer = new ComputeBuffer(colourKeys.Length, sizeof(float) * 5);

                colourBuffer.SetData(colors);
                vertexBuffer.SetData(mesh.vertices);
                colourKeysBuffer.SetData(colourKeys);

                computeShader.SetBuffer(0, "Colour", colourBuffer);
                computeShader.SetBuffer(0, "Vertices", vertexBuffer);
                computeShader.SetBuffer(0, "Keys", colourKeysBuffer);

                computeShader.Dispatch(0, (int)Mathf.Ceil(mesh.vertices.Length / 256f), 1, 1);

                colourBuffer.GetData(colors);

                mesh.colors = colors;

                colourBuffer.Dispose();
                vertexBuffer.Dispose();
                colourKeysBuffer.Dispose();
            }
        }

        stopwatch.Stop();
        UnityEngine.Debug.Log("Gradient CS: " + stopwatch.Elapsed.ToString("mm\\:ss\\.fffff"));
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 50, 100, 30), "Gradient CS"))
            GenerateRegions();
    }
}
