using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.IO;
using UnityEngine.Networking;
using System;

public class SongManager : MonoBehaviour
{
    public static SongManager Instance;
    public AudioSource audioSource;
    public Lane[] lanes;
    public float songDelayInSeconds;
    public double marginOfError;
    public int inputDelayInMilliseconds;
    public string fileLocation;
    public float noteTime;
    public float noteSpawnY;
    public float noteTapY;
    public float noteDespawnY
    {
        get
        {
            return noteTapY - (noteSpawnY - noteTapY);
        }
    }

    public static MidiFile midiFile;

    void Start()
    {
        Instance = this;
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

    private void ReadFromFile()
    {
        midiFile = MidiFile.Read(Application.streamingAssetsPath + "/" + fileLocation);
        GetDataFromMidi();
    }

    public void GetDataFromMidi()
    {
        var tempoMap = midiFile.GetTempoMap();
        var notes = midiFile.GetNotes();
        var array = new Melanchall.DryWetMidi.Interaction.Note[notes.Count];
        notes.CopyTo(array, 0);

        foreach (var tempoChange in tempoMap.GetTempoChanges())
        {
            var tempo = tempoChange.Value.BeatsPerMinute;
            var time = tempoChange.Time;
            Debug.Log($"Tempo: {tempo} BPM, Time: {time}");
        }

        DistributeNotesToLanes(array);
        foreach (var lane in lanes) lane.SetTimeStamps(array);

        Invoke(nameof(StartSong), songDelayInSeconds);
    }

    public void StartSong()
    {
        audioSource.Play();
    }

    public static double GetAudioSourceTime()
    {
        return (double)Instance.audioSource.timeSamples / Instance.audioSource.clip.frequency;
    }

    public void DistributeNotesToLanes(Melanchall.DryWetMidi.Interaction.Note[] allNotes)
    {
        var noteValues = new HashSet<int>();
        foreach (var note in allNotes)
        {
            noteValues.Add((int)note.NoteName);
        }

        var sortedNotes = new List<int>(noteValues);
        sortedNotes.Sort();

        int laneCount = lanes.Length;

        for (int i = 0; i < sortedNotes.Count; i++)
        {
            int laneIndex = Mathf.Clamp(i * laneCount / sortedNotes.Count, 0, laneCount - 1);
            lanes[laneIndex].noteRestriction = (Melanchall.DryWetMidi.MusicTheory.NoteName)sortedNotes[i];
        }
    }

    void Update()
    {
        
    }
}