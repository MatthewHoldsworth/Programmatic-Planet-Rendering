using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cube : MonoBehaviour
{
    [SerializeField, Range(2, 512)]
    int resolution = 2;
    [SerializeField, Range(1, 10)]
    int radius = 1;
    [SerializeField]
    bool useNoise = false;
    [SerializeField]
    NoiseFilter noiseFilter;
    [SerializeField]
    int LODRes;

    Vector3[] vertices;
    int[] triangles;
    Mesh mesh;

    // Start is called before the first frame update
    public void OnValidate()
    {
        
        if (mesh == null)
        {
            gameObject.GetComponent<MeshFilter>().sharedMesh = new Mesh();
            mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        }
        

        Vector3[] directions = { Vector3.up};

        vertices = new Vector3[resolution * resolution * 6];

        triangles = new int[(resolution - 1) * (resolution - 1) * 6 * 6];

        for (int i = 0; i < directions.Length; i++)
        {
            GeneratePlane(directions[i], i);
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        //Debug.Log(Time.unscaledTime);
        //Debug.Log(mesh.vertices.Length);

        //Debug.Log(mesh.triangles.Length);
    }

    // Update is called once per frame
    void GeneratePlane(Vector3 localUp, int index)
    {
        //Perpendicular to localUp
        Vector3 axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        //Normal of axisA and localUp
        Vector3 axisB = Vector3.Cross(localUp, axisA);
        //Both used as the x and y of the current position on the plane

        //Vector3[] vertices = new Vector3[resolution * resolution];
        //int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = (resolution - 1) * (resolution - 1) * 6 * index;
        int verIndex = resolution * resolution * index;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + (y * resolution) + verIndex;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                //Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                switch (useNoise)
                {
                    case true:
                        vertices[i] = pointOnUnitCube * noiseFilter.Elevate(pointOnUnitCube) * radius;
                        break;
                    case false:
                        vertices[i] = pointOnUnitCube * radius;
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
    }
}
