using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

[System.Serializable]
public class VoronoiCS : MonoBehaviour
{
    public ComputeShader computeShader;
    [SerializeField, Range(1, 1000)]
    int regions = 10;

    [SerializeField, Range(0f, 3f)]
    float seaBed;
    [SerializeField]
    Color oceanFloor;


    struct Node
    {
        int verIndex;
        Color colour;
    };

    // Start is called before the first frame update
    public void GenerateBiomes()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        Mesh mesh;

        List<Vector3> allVertices = new List<Vector3>();

        for (int j = 0; j < transform.childCount; j++)
        {
            if (transform.GetChild(j).GetComponent<MeshFilter>() != null)
            {
                mesh = transform.GetChild(j).GetComponent<MeshFilter>().sharedMesh;
                allVertices.AddRange(mesh.vertices);
            }
        }

        Vector3[] nodes = new Vector3[regions + 1];
        Color[] nodeColors = new Color[regions + 1];
        nodeColors[0] = oceanFloor;

        for (int i = 1; i < regions+1; i++)
        {
            nodes[i] = allVertices[Random.Range(0, allVertices.Count - 1)];

            nodeColors[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        }

        for (int j = 0; j < transform.childCount; j++)
        {
            if (transform.GetChild(j).GetComponent<MeshFilter>() != null)
            {
                mesh = transform.GetChild(j).GetComponent<MeshFilter>().sharedMesh;

                ComputeBuffer colourBuffer = new ComputeBuffer(mesh.vertices.Length, sizeof(float) * 4);
                ComputeBuffer vertexBuffer = new ComputeBuffer(mesh.vertices.Length, sizeof(float) * 3);
                ComputeBuffer nodeBuffer = new ComputeBuffer(nodes.Length, sizeof(float) * 3);
                ComputeBuffer nodeCBuffer = new ComputeBuffer(nodeColors.Length, sizeof(float) * 4);

                Color[] color = new Color[mesh.vertices.Length];

                colourBuffer.SetData(color);
                vertexBuffer.SetData(mesh.vertices);
                nodeBuffer.SetData(nodes);
                nodeCBuffer.SetData(nodeColors);

                computeShader.SetBuffer(0, "Colour", colourBuffer);
                computeShader.SetBuffer(0, "Vertices", vertexBuffer);
                computeShader.SetBuffer(0, "Nodes", nodeBuffer);
                computeShader.SetBuffer(0, "NodesColour", nodeCBuffer);

                computeShader.SetFloat("seaBed", seaBed);

                computeShader.Dispatch(0, (int)Mathf.Ceil(mesh.vertices.Length / 256f), 1, 1);

                colourBuffer.GetData(color);

                /*for(int i = 0; i<color.Length; i++)
                {
                    Debug.Log(color[i]);
                }*/

                mesh.colors = color;

                colourBuffer.Dispose();
                vertexBuffer.Dispose();
                nodeBuffer.Dispose();
                nodeCBuffer.Dispose();
            }
        }

        stopwatch.Stop();
        UnityEngine.Debug.Log("Voronoi CS: " + stopwatch.Elapsed.ToString("mm\\:ss\\.fffff"));
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 100, 100, 30), "Voronoi CS "))
            GenerateBiomes();
    }
}
