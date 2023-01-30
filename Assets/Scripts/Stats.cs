using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.Profiling;

public class Stats : MonoBehaviour
{
    struct Frame
    {
        public float fps;
        public float time;

        public Frame (float ifps, float itime){
            fps = ifps;
            time = itime;
        }
    };

    [SerializeField]
    string filename = "ScenePerformance";
    [SerializeField]
    int range = 100;

    string elapsedTime;

    ProfilerRecorder _meshMemoryRecorder;
    ProfilerRecorder _totalReservedMemoryRecorder;

    List<Frame> frames = new List<Frame>();

    void Start()
    {
        _meshMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Mesh Memory");
        _totalReservedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Reserved Memory");

        frames.Clear();
    }

    void CollectData()
    {
        
        frames.Add(new Frame(1.0f/Time.unscaledDeltaTime,Time.time));
    }

    string ToCSV()
    {
        
        string contents = "Index,SceneTime(seconds),FramesPerSecond,TotalAllocatedMemory(kB),MeshMemory(kB),ElapsedTime(seconds//milliseconds)";
        
        if (range < frames.Count) {
            contents += '\n' + "0" + ',' + frames[frames.Count - range].time.ToString() + ',' + frames[frames.Count - range].fps.ToString() + ',' 
                + (_totalReservedMemoryRecorder.LastValue / 8 / 1000).ToString() + ',' + (_meshMemoryRecorder.LastValue / 8 / 1000).ToString() + ',' + elapsedTime;
            int count = 0;
            for (int i = frames.Count - range + 1; i < frames.Count; i++){
                count++;
                contents += '\n' + count.ToString() + ',' + frames[i].time.ToString() + ',' + frames[i].fps.ToString();
            }
        }
        else {
            contents += '\n' + "0" + ',' + frames[0].time.ToString() + ',' + frames[0].fps.ToString() + ',' + 
                (_totalReservedMemoryRecorder.LastValue / 8/1000).ToString() + ',' + (_meshMemoryRecorder.LastValue / 8 / 1000).ToString() + ',' + elapsedTime;

            for (int i = 1; i < frames.Count; i++){
                contents += '\n' + i.ToString() + ',' + frames[i].time.ToString() + ',' + frames[i].fps.ToString();
            }
        }

        _meshMemoryRecorder.Dispose();
        _totalReservedMemoryRecorder.Dispose();

        return contents;
    }

    public void SetElapsedTime(string time)
    {
        elapsedTime = time;
    }

    // Update is called once per frame
    void Update()
    {
        CollectData();
    }

    public void OnApplicationQuit()
    {
        string content = ToCSV();
        //Creates the file or overwrites one if it already exists
        File.WriteAllText("Assets/Data/" + filename + ".csv", content);
    }
}
