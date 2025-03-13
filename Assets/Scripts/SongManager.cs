using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.IO;
using UnityEngine.Networking;
using System;
using SimpleFileBrowser;
using Unity.VisualScripting;

public class SongManager : MonoBehaviour
{
    public static SongManager Instance;
    //public AudioSource audioSource;
    public Lane[] lanes;
    public float songDelayInSeconds;
    public double marginOfError; // in seconds

    public int inputDelayInMilliseconds;


    public string fileLocation;
    public float noteTime;
    public float noteSpawnY;
    public float noteTapY;

    //public List<double> debugNoteTimes = new List<double>();
    public float beatDuration; // Thoi gian cua moi beat
    public float measureDuration; // Thoi gian c?a moi o nhip
    public float measure;
    public int beatsPerMeasure = 4; // Mac dinh la 4/4
    public bool isPlaying = false;
    private static double pausedTime = 0;

    public float noteDespawnY
    {
        get
        {
            return noteTapY - (noteSpawnY - noteTapY);
        }
    }

    public static MidiFile midiFile;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
        fileLocation = "";
        if (Application.streamingAssetsPath.StartsWith("http://") || Application.streamingAssetsPath.StartsWith("https://"))
        {
            StartCoroutine(ReadFromWebsite());
        }
        else
        {
            ReadFromFile();
        }
    }

    private IEnumerator ReadFromWebsite()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + fileLocation))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                byte[] results = www.downloadHandler.data;
                using (var stream = new MemoryStream(results))
                {
                    midiFile = MidiFile.Read(stream);
                    GetDataFromMidi();
                }
            }
        }
    }

    //Open File Midi Select
    public void LoadSelectedMidi()
    {
        if (!string.IsNullOrEmpty(fileLocation))
        {
            Debug.Log("Loading MIDI file from: " + fileLocation);
            midiFile = MidiFile.Read(fileLocation);
            GetDataFromMidi(); // X? lý file MIDI sau khi load
        }
        else
        {
            Debug.LogError("No MIDI file selected.");
        }
    }

    public void OpenFilePicker()
    {
        SimpleFileBrowser.FileBrowser.SetFilters(true, new SimpleFileBrowser.FileBrowser.Filter("MIDI Files", ".mid"));
        SimpleFileBrowser.FileBrowser.SetDefaultFilter(".mid");
        SimpleFileBrowser.FileBrowser.ShowLoadDialog((paths) =>
        {
            fileLocation = paths[0]; // L?u ???ng d?n file
            Debug.Log("Selected MIDI file: " + fileLocation);
            ReadFromFile(); // Ch? load file sau khi ch?n
            MidiTimeLine.Instance.ResetAfterOpenFile();
        },
        () => { Debug.Log("User canceled file selection."); },
        SimpleFileBrowser.FileBrowser.PickMode.Files, false, null, "Select MIDI File", "Load");
    }

    private void ReadFromFile()
    {
        //midiFile = MidiFile.Read(Application.streamingAssetsPath + "/" + fileLocation);
        //GetDataFromMidi();

        string fullPath = fileLocation;

        // N?u fileLocation không ph?i là ???ng d?n tuy?t ??i, gán thêm StreamingAssetsPath
        if (!Path.IsPathRooted(fileLocation))
        {
            fullPath = Path.Combine(Application.streamingAssetsPath, fileLocation);
        }

        if (File.Exists(fullPath))
        {
            midiFile = MidiFile.Read(fullPath);
            GetDataFromMidi();
        }
        else
        {
            Debug.LogError("MIDI file not found at: " + fullPath);
        }
    }
    //public void GetDataFromMidi()
    //{
    //    var notes = midiFile.GetNotes();
    //    var array = new Melanchall.DryWetMidi.Interaction.Note[notes.Count];
    //    notes.CopyTo(array, 0);

    //    // Tu dong phan tich cao do va gan Lane
    //    DistributeNotesToLanes(array);

    //    foreach (var lane in lanes) lane.SetTimeStamps(array);

    //    Invoke(nameof(StartSong), songDelayInSeconds);
    //}

    public void GetDataFromMidi()
    {
        //debugNoteTimes.Clear();
        var tempoMap = midiFile.GetTempoMap();
        var notes = midiFile.GetNotes();
        var array = new Melanchall.DryWetMidi.Interaction.Note[notes.Count];
        notes.CopyTo(array, 0);

        // Ki?m tra tempo và log thông tin
        foreach (var tempoChange in tempoMap.GetTempoChanges())
        {
            var tempo = tempoChange.Value.BeatsPerMinute;
            var time = tempoChange.Time;
            beatDuration = (float)(60f / tempo);
            measureDuration = beatDuration * beatsPerMeasure;
            Debug.Log($"Tempo: {tempo} BPM, Time: {time}, beatDuration: {beatDuration}s, measureDuration: {measureDuration}s");
        }

        foreach (var lane in lanes)
        {
            lane.ClearNotes();
        }

        //foreach (var note in array) 
        //{
        //    var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, midiFile.GetTempoMap());
        //    double startTime = (double)metricTimeSpan.Minutes * 60 + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000;
        //    debugNoteTimes.Add(startTime);
        //}

        // Phân tích và gán vào lanes
        DistributeNotesToLanes(array);
        //foreach (var lane in lanes) lane.SetTimeStamps(array);
        foreach (var lane in lanes)
        {
            lane.SetTimeStamps(array);
            lane.SpawnAllNotes(); // G?i hàm Update ?? spawn t?t c? các n?t
        }

        //Invoke(nameof(StartSong), songDelayInSeconds);
        isPlaying = false;
        startTime = 0;
        pausedTime = 0;
        MidiTimeLine.Instance.ResetTimeline();
    }

    private static double startTime; // Bi?n ?? l?u th?i gian b?t ??u

    public void StartSong()
    {
        //startTime = Time.time; // L?u th?i gian b?t ??u
        //audioSource.Play();
        if (midiFile == null) return; // ??m b?o ?ã load MIDI
        startTime = Time.time; // Kh?i ??ng l?i t? ??u
        isPlaying = true;
    }

    public void PlayPauseSong()
    {
        if (!isPlaying)
        {
            //if (startTime == 0) // N?u ch?a t?ng ch?y tr??c ?ó
            //{
            //    StartSong();
            //}
            //else
            //{
            //    startTime = Time.time - pausedTime; // Resume t? th?i ?i?m d?ng
            //}
            startTime = Time.time - pausedTime;
            isPlaying = true;
        }
        else
        {
            isPlaying = false;
            pausedTime = Time.time - startTime; // L?u l?i th?i gian ?ã trôi qua
        }
    }

    public void StopSong()
    {
        isPlaying = false;
        startTime = 0; // Reset v? th?i gian b?t ??u
        pausedTime = 0;

        MidiTimeLine.Instance.ResetTimeline();
    }

    public void FastForward()
    {
        if (midiFile == null) return; // ??m b?o MIDI ?ã ???c load

        // Tính toán th?i gian m?i
        double newTime = GetAudioSourceTime() + measureDuration;

        // N?u ?ang d?ng, c?p nh?t pausedTime
        if (!isPlaying)
        {
            pausedTime = newTime;
        }
        else
        {
            startTime = Time.time - newTime; // C?p nh?t th?i gian b?t ??u
        }

        // G?i c?p nh?t Timeline
        MidiTimeLine.Instance.UpdateTimeLinePosition(newTime);
    }

    public void Rewind()
    {
        if (midiFile == null) return; // ??m b?o MIDI ?ã ???c load

        // Tính toán th?i gian m?i (tua ng??c)
        double newTime = GetAudioSourceTime() - measureDuration;

        // ??m b?o không tua v? th?i gian âm
        newTime = Mathf.Max(0, (float)newTime);

        // N?u ?ang d?ng, c?p nh?t pausedTime
        if (!isPlaying)
        {
            pausedTime = newTime;
        }
        else
        {
            startTime = Time.time - newTime; // C?p nh?t th?i gian b?t ??u
        }

        // G?i c?p nh?t Timeline
        MidiTimeLine.Instance.UpdateTimeLinePosition(newTime);
    }

    public static double GetAudioSourceTime()
    {
        //return (double)Instance.audioSource.timeSamples / Instance.audioSource.clip.frequency;
        if (!Instance.isPlaying) return pausedTime;
        return Time.time - startTime; // Tính th?i gian ?ã trôi qua k? t? khi bài hát b?t ??u
    }

    public void DistributeNotesToLanes(Melanchall.DryWetMidi.Interaction.Note[] allNotes)
    {
        // Lay danh sach cao do duy nhat va sap xep tang dan
        var noteValues = new HashSet<int>();
        foreach (var note in allNotes)
        {
            noteValues.Add((int)note.NoteName);
        }

        var sortedNotes = new List<int>(noteValues);
        sortedNotes.Sort();

        // Phan chia cao do vao tung lane
        int laneCount = lanes.Length;

        for (int i = 0; i < sortedNotes.Count; i++)
        {
            int laneIndex = Mathf.Clamp(i * laneCount / sortedNotes.Count, 0, laneCount - 1);
            lanes[laneIndex].noteRestriction = (Melanchall.DryWetMidi.MusicTheory.NoteName)sortedNotes[i];
        }
    }

    //void OnDrawGizmos()
    //{
    //    if (debugNoteTimes == null || debugNoteTimes.Count == 0) return;

    //    Gizmos.color = Color.red;
    //    float yOffset = 2f; // Kho?ng cách gi?a các n?t trên timeline
    //    float startX = -5f; // ?i?m b?t ??u v? timeline

    //    for (int i = 0; i < debugNoteTimes.Count; i++)
    //    {
    //        float xPosition = startX + (float)debugNoteTimes[i] * 0.5f; // Tính v? trí theo th?i gian
    //        Gizmos.DrawSphere(new Vector3(xPosition, yOffset, 0), 0.2f);
    //    }

    //    Gizmos.color = Color.green;
    //    float currentTimeX = startX + (float)GetAudioSourceTime() * 0.5f;
    //    Gizmos.DrawLine(new Vector3(currentTimeX, yOffset - 0.5f, 0), new Vector3(currentTimeX, yOffset + 0.5f, 0));
    //}

    void Update()
    {

    }
}
