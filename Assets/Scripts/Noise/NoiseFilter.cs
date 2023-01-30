using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

[System.Serializable]
public class NoiseFilter
{
    [System.Serializable]
    struct NoiseSettings
    {
        [SerializeField, Range(0f, 3f)]
        public float roughness;
        [SerializeField, Range(0f, 100f)]
        public float offset;
        [SerializeField, Range(1, 10)]
        public int layers;
        [SerializeField, Range(0.1f, 1f)]
        public float amplitude;
    }

    Noise noise = new Noise();
    [SerializeField]
    NoiseSettings[] noiseSettings;

    [SerializeField, Range(1f, 3f)] 
    float min = 1f;

    // Start is called before the first frame update
    public float Elevate(Vector3 vector)
    {

        Stopwatch stopwatch = new Stopwatch();
        if (Application.isPlaying)
        {
            stopwatch.Start();
        }
        float elevation = 0f;
        float baseLevel = 0f;

        foreach (NoiseSettings n in noiseSettings)
        {

            Vector3 offset = new Vector3(n.offset, n.offset, n.offset);
            float noiseValue = 0f;
            float amplitude = n.amplitude;
            float localRoughness = n.roughness;
            if (elevation > 0f)
            {
                for (int i = 0; i < n.layers; i++)
                {
                    noiseValue += (noise.Evaluate((vector + offset) * localRoughness) + 1) / 2 * amplitude;
                    //noiseValue += Mathf.Abs(noise.Evaluate((vector + offset) * localRoughness)) * amplitude;
                    localRoughness *= 2;
                    amplitude *= 0.5f;
                    noiseValue *= baseLevel;
                }
            }
            else
            {
                for (int i = 0; i < n.layers; i++)
                {
                    noiseValue += (noise.Evaluate((vector + offset) * localRoughness) + 1) / 2 * amplitude;
                    //noiseValue += Mathf.Abs(noise.Evaluate((vector + offset) * localRoughness)) * amplitude;
                    localRoughness *= 2;
                    amplitude *= 0.5f;
                    baseLevel = noiseValue;
                }
            }
            elevation += (noiseValue);
            //Debug.Log(elevation);

        }
        elevation = Mathf.Clamp(elevation + 1, min, float.MaxValue);
        if (Application.isPlaying)
        { 
            stopwatch.Stop();
            UnityEngine.Debug.Log("Noise time: " + stopwatch.Elapsed.ToString("mm\\:ss\\.fffff"));
        }

        return elevation;
    }
}
