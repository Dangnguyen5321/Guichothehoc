using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lane : MonoBehaviour
{
    public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction;
    public KeyCode input;
    public GameObject notePrefab;
    List<Note> notes = new List<Note>();
    public List<double> timeStamps = new List<double>();
    public List<double> durations = new List<double>(); // Luu thoi luong not

    int spawnIndex = 0;
    int inputIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void SetTimeStamps(Melanchall.DryWetMidi.Interaction.Note[] array)
    {
        foreach (var note in array)
        {
            if (note.NoteName == noteRestriction)
            {
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, SongManager.midiFile.GetTempoMap());
                double startTime = (double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f;
                timeStamps.Add(startTime);

                // Lay thoi luong not
                var duration = TimeConverter.ConvertTo<MetricTimeSpan>(note.Length, SongManager.midiFile.GetTempoMap());
                double noteDuration = (double)duration.Minutes * 60f + duration.Seconds + (double)duration.Milliseconds / 1000f;
                durations.Add(noteDuration); // Luu thoi luong

                //Debug.Log($"Note: {note.NoteName}, Start Time: {startTime}, Duration: {noteDuration}");
                Debug.Log($"Lane -> Assigned Duration: {durations[spawnIndex]}");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnIndex < timeStamps.Count)
        {
            if (SongManager.GetAudioSourceTime() >= timeStamps[spawnIndex] - SongManager.Instance.noteTime)
            {
                var note = Instantiate(notePrefab, transform);
                notes.Add(note.GetComponent<Note>());
                // Truyền thời gian và thời lượng vào Note
                var noteComponent = note.GetComponent<Note>();
                note.GetComponent<Note>().assignedTime = (float)timeStamps[spawnIndex];
                noteComponent.assignedDuration = (float)durations[spawnIndex]; // Truyền thời lượng nốt
                //Debug.Log($"[Lane] Spawned Note {spawnIndex} | StartTime: {timeStamps[spawnIndex]}, Duration: {durations[spawnIndex]}");
                spawnIndex++;
            }
        }

        if (inputIndex < timeStamps.Count)
        {
            double timeStamp = timeStamps[inputIndex];
            double marginOfError = SongManager.Instance.marginOfError;
            double audioTime = SongManager.GetAudioSourceTime() - (SongManager.Instance.inputDelayInMilliseconds / 1000.0);

            if (Input.GetKeyDown(input))
            {
                if (Math.Abs(audioTime - timeStamp) < marginOfError)
                {
                    Hit();
                    print($"Hit on {inputIndex} note");
                    Destroy(notes[inputIndex].gameObject);
                    inputIndex++;
                }
                else
                {
                    print($"Hit inaccurate on {inputIndex} note with {Math.Abs(audioTime - timeStamp)} delay");
                }
            }
            if (timeStamp + marginOfError <= audioTime)
            {
                Miss();
                print($"Missed {inputIndex} note");
                inputIndex++;
            }
        }       
    
    }
    private void Hit()
    {
        ScoreManager.Hit();
    }
    private void Miss()
    {
        ScoreManager.Miss();
    }
}
