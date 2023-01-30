using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class GradientRegions : MonoBehaviour
{
    [SerializeField]
    Gradient gradient;

    // Update is called once per frame
    void GenerateRegions()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();



        Mesh mesh;

        for (int j = 0; j < transform.childCount; j++)
        {
            if (transform.GetChild(j).GetComponent<MeshFilter>() != null)
            {
                mesh = transform.GetChild(j).GetComponent<MeshFilter>().sharedMesh;
                Color[] colors = new Color[mesh.vertices.Length];
                for (int i = 0; i < mesh.vertices.Length; i++)
                {
                    float distance = Vector3.Distance(mesh.vertices[i], transform.position);
                    colors[i] = gradient.Evaluate((distance - 1));
                }
                mesh.colors = colors;
            }
        }

        stopwatch.Stop();
        UnityEngine.Debug.Log("Gradient C#: " + stopwatch.Elapsed.ToString("mm\\:ss\\.fffff"));
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 50, 100, 30), "Gradient C#"))
            GenerateRegions();
    }
}
