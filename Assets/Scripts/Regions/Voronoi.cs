using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

[System.Serializable]
public class Voronoi : MonoBehaviour
{
    [SerializeField, Range(1, 1000)]
    int regions = 1;

    [SerializeField, Range(0f, 3f)]
    float seaBed;
    [SerializeField]
    Color oceanFloor;

    // Start is called before the first frame update
    public void GenerateRegions()
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

        Vector3[] nodes = new Vector3[regions+1];
        Color[] nodeColors = new Color[regions + 1];

        nodeColors[0] = oceanFloor;

        for (int i = 1; i < regions+1; i++)
        {
            nodes[i] = allVertices[Random.Range(0, allVertices.Count-1)];

            nodeColors[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        }


        for (int j = 0; j < transform.childCount; j++)
        {
            if (transform.GetChild(j).GetComponent<MeshFilter>() != null)
            {
                mesh = transform.GetChild(j).GetComponent<MeshFilter>().sharedMesh;

                Color[] colors = new Color[mesh.vertices.Length];

                for (int i = 0; i < mesh.vertices.Length; i++)
                {
                    int index = GetClosestNode(mesh.vertices[i], nodes, mesh);
                    colors[i] = nodeColors[index];
                }

                mesh.colors = colors;
                mesh.RecalculateBounds();
            }
        }

        stopwatch.Stop();
        UnityEngine.Debug.Log("Voronoi C#: " + stopwatch.Elapsed.ToString("mm\\:ss\\.fffff"));
    }
    
    int GetClosestNode(Vector3 vector, Vector3[] nodes, Mesh mesh)
    {
        float nearest = float.MaxValue;
        int index = 0;
        if (Vector3.Distance(vector, new Vector3(0f,0f,0f))>= seaBed) {
            for (int i = 1; i < nodes.Length; i++)
            {
                if (Vector3.Distance(vector, nodes[i]) <= nearest)
                {
                    nearest = Vector3.Distance(vector, nodes[i]);
                    index = i;
                }
            }
        }
        else
        {
            index = 0;
        }
        return index;
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 100, 100, 30), "Vornoi C#"))
            GenerateRegions();
    }
}
