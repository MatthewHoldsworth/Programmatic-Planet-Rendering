using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Unity.Profiling;

public class Counters : MonoBehaviour
{
    //the interval at which it collects data in seconds
    [SerializeField]
    float interval;
    float fps;
    float count = 1;
    float memory;
    //unity profile recorder
    ProfilerRecorder gcMemoryRecorder;

    void Start()
    {
        //sets up a memory profile recorder
        gcMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory");
    }

    // Update is called once per frame
    void Update()
    {
        //enumerates count
        count += Time.deltaTime;
        if (count >= interval)
        {
            //resets the count
            count = 0;
            //calculate fps by time between last frame, unscaled
            fps = 1 / Time.unscaledDeltaTime;
            //get the last value of memory recorder and convert it into bytes, then kilobytes
            memory = gcMemoryRecorder.LastValue / 8 / 1024 ;
        }
    }

    //creates and displays graphical interafce of the counters
    void OnGUI()
    {
        //creatse a box with the text of the counts
        GUI.Box(new Rect(160, 10, 100, 25), "fps: " + fps.ToString());
        GUI.Box(new Rect(310, 10, 100, 25), "Memory: " + memory.ToString() + " kB");
    }
}
