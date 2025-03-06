using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lane : MonoBehaviour
{
    public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction;
    public KeyCode input; // Không còn dùng nữa nhưng giữ lại nếu bạn muốn dùng sau
    public GameObject notePrefab;
    List<Note> notes = new List<Note>();
    public List<double> timeStamps = new List<double>();
    public List<double> durations = new List<double>();

    int spawnIndex = 0;

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

                var duration = TimeConverter.ConvertTo<MetricTimeSpan>(note.Length, SongManager.midiFile.GetTempoMap());
                double noteDuration = (double)duration.Minutes * 60f + duration.Seconds + (double)duration.Milliseconds / 1000f;
                durations.Add(noteDuration);

                Debug.Log($"Lane -> Assigned Duration: {durations[spawnIndex]}");
            }
        }
    }

    void Update()
    {
        if (spawnIndex < timeStamps.Count)
        {
            if (SongManager.GetAudioSourceTime() >= timeStamps[spawnIndex] - SongManager.Instance.noteTime)
            {
                var note = Instantiate(notePrefab, transform);
                notes.Add(note.GetComponent<Note>());
                var noteComponent = note.GetComponent<Note>();
                note.GetComponent<Note>().assignedTime = (float)timeStamps[spawnIndex];
                noteComponent.assignedDuration = (float)durations[spawnIndex];
                spawnIndex++;
            }
        }
        
        if (Input.GetMouseButtonDown(0)) // Nhấp chuột trái
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction);
            foreach (var hit in hits)
            {
                Note note = hit.collider?.GetComponent<Note>();
                if (note != null)
                {
                    Note.score++;
                    Debug.Log($"Tile destroyed! Current score: {Note.score}");
                    Destroy(note.gameObject);
                }
            }
        }
    }
}