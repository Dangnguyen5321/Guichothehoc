using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Lane : MonoBehaviour
{
        public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction;
    public KeyCode input;
    public GameObject notePrefab;
    List<Note> notes = new List<Note>();
    //public List<GameObject> noteObjects = new List<GameObject>();
    public List<double> timeStamps = new List<double>();
    public List<double> durations = new List<double>(); // Luu thoi luong not

    int spawnIndex = 0;
    int inputIndex = 0;

    public float zoomFactor = 1f; // Giá trị mặc định của zoom
    public static float minZoom = 1f, maxZoom = 15f;

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
                //Debug.Log($"Lane -> Assigned Duration: {durations[spawnIndex]}");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (spawnIndex < timeStamps.Count)
        //{
        //    if (SongManager.GetAudioSourceTime() >= timeStamps[spawnIndex] - SongManager.Instance.noteTime)
        //    {
        //        var note = Instantiate(notePrefab, transform);
        //        notes.Add(note.GetComponent<Note>());
        //        // Truy?n th?i gian và th?i l??ng vào Note
        //        var noteComponent = note.GetComponent<Note>();
        //        note.GetComponent<Note>().assignedTime = (float)timeStamps[spawnIndex];
        //        noteComponent.assignedDuration = (float)durations[spawnIndex]; // Truy?n th?i l??ng n?t
        //        //Debug.Log($"[Lane] Spawned Note {spawnIndex} | StartTime: {timeStamps[spawnIndex]}, Duration: {durations[spawnIndex]}");
        //        spawnIndex++;
        //    }
        //}



        //if (inputIndex < timeStamps.Count)
        //{
        //    double timeStamp = timeStamps[inputIndex];
        //    double marginOfError = SongManager.Instance.marginOfError;
        //    double audioTime = SongManager.GetAudioSourceTime() - (SongManager.Instance.inputDelayInMilliseconds / 1000.0);

        //    if (Input.GetKeyDown(input))
        //    {
        //        if (Math.Abs(audioTime - timeStamp) < marginOfError)
        //        {
        //            Hit();
        //            print($"Hit on {inputIndex} note");
        //            Destroy(notes[inputIndex].gameObject);
        //            inputIndex++;
        //        }
        //        else
        //        {
        //            print($"Hit inaccurate on {inputIndex} note with {Math.Abs(audioTime - timeStamp)} delay");
        //        }
        //    }
        //    if (timeStamp + marginOfError <= audioTime)
        //    {
        //        Miss();
        //        print($"Missed {inputIndex} note");
        //        inputIndex++;
        //    }
        //}
        //

        HandleZoomInput();
    }
    public void SpawnAllNotes()
    {
        foreach (var timeStamp in timeStamps)
        {
            if (spawnIndex < timeStamps.Count)
            {
                // Instantiate not
                var note = Instantiate(notePrefab, transform);
                notes.Add(note.GetComponent<Note>());

                // Gán thoi gian và thoi luong cho not
                var noteComponent = note.GetComponent<Note>();
                noteComponent.assignedTime = (float)timeStamps[spawnIndex];
                noteComponent.assignedDuration = (float)durations[spawnIndex];

                //// Tính vi trí hien thi not theo thoi gian bat dau
                //float noteY = SongManager.Instance.noteTapY + (noteComponent.assignedTime * SongManager.Instance.noteTime * zoomFactor);
                ////note.transform.localPosition = new Vector3(0, noteY, 0); // Ch? thay ??i v? trí Y
                //note.transform.localPosition = new Vector3(0, noteY + (noteComponent.assignedDuration * zoomFactor / 2), 0);
                ////note.transform.localScale = new Vector3(1, noteComponent.assignedDuration, 1); // Thay ??i chi?u dài theo th?i l??ng

                spawnIndex++;
            }
        }

        UpdateNotePositions();
    }

    //void HandleZoomInput()
    //{
    //    if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
    //    {
    //        float scroll = Input.GetAxis("Mouse ScrollWheel");
    //        if (scroll != 0)
    //        {
    //            zoomFactor = Mathf.Clamp(zoomFactor + scroll * zoomSpeed, minZoom, maxZoom);
    //            UpdateNotePositions();
    //        }
    //    }

    //    if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus)) // Phím +
    //    {
    //        zoomFactor = Mathf.Clamp(zoomFactor + zoomSpeed, minZoom, maxZoom);
    //        UpdateNotePositions();
    //    }

    //    if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus)) // Phím -
    //    {
    //        zoomFactor = Mathf.Clamp(zoomFactor - zoomSpeed, minZoom, maxZoom);
    //        UpdateNotePositions();
    //    }
    //}
    void HandleZoomInput()
    {
        if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus)) // Phím +
        {
            zoomFactor = Mathf.Min(zoomFactor + 1f, maxZoom);
            UpdateNotePositions();
        }

        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus)) // Phím -
        {
            zoomFactor = Mathf.Max(zoomFactor - 1f, minZoom);
            UpdateNotePositions();
        }
    }
    void UpdateNotePositions()
    {
        foreach (var note in notes)
        {
            if (note == null) continue;

            float noteY = SongManager.Instance.noteTapY + (note.assignedTime * SongManager.Instance.noteTime * zoomFactor);
            note.transform.localPosition = new Vector3(0, noteY + (note.assignedDuration * zoomFactor / 2), 0);
        }
    }

    public void ClearNotes()
    {
        foreach (var note in notes)
        {
            if (note != null)
                Destroy(note.gameObject);
        }

        notes.Clear();
        timeStamps.Clear();
        durations.Clear();
        spawnIndex = 0;
        inputIndex = 0;
    }


    //public void ClearNotes()
    //{
    //    foreach (GameObject note in noteObjects)
    //    {
    //        Destroy(note); // Xóa tất cả nốt đã spawn trước đó
    //    }

    //    noteObjects.Clear(); // Xóa danh sách nốt đã lưu (nếu có)
    //    timeStamps.Clear(); // Xóa timestamp cũ

    //    Debug.Log("Cleared notes in lane: " + gameObject.name);
    //}

    //public void UpdateNotePositions()
    //{
    //    foreach (Transform note in transform)
    //    {
    //        Note noteComponent = note.GetComponent<Note>();
    //        if (noteComponent == null) continue;

    //        float originalY = noteComponent.assignedTime * SongManager.Instance.noteTime;
    //        float newScaleY = noteComponent.assignedDuration * zoomFactor;
    //        float newY = originalY * zoomFactor; // C?p nh?t v? trí theo zoom

    //        note.localScale = new Vector3(1, newScaleY, 1);
    //        note.localPosition = new Vector3(note.localPosition.x, newY, note.localPosition.z);
    //    }
    //}
    //private void Hit()
    //{
    //    ScoreManager.Hit();
    //}
    //private void Miss()
    //{
    //    ScoreManager.Miss();
    //}
}