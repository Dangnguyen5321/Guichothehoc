using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    double timeInstantiated;
    public float assignedTime;
    public float assignedDuration;
    public static int score = 0; // Biến tĩnh để lưu điểm số

    void Start()
    {
        timeInstantiated = SongManager.GetAudioSourceTime();
        Debug.Log($"[Note] Spawned - Assigned Time: {assignedTime}, Assigned Duration: {assignedDuration}");
    }

    void Update()
    {
        double timeSinceInstantiated = SongManager.GetAudioSourceTime() - timeInstantiated;
        float t = (float)(timeSinceInstantiated / (SongManager.Instance.noteTime * 2));

        if (t > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(Vector3.up * SongManager.Instance.noteSpawnY, Vector3.up * SongManager.Instance.noteDespawnY, t);
            UpdateNoteAppearance();
            GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    void OnMouseDown()
    {
        score++; // Tăng điểm khi click
        Debug.Log($"Tile destroyed! Current score: {score}");
        Destroy(gameObject); // Hủy nốt khi click
    }

    private void UpdateNoteAppearance()
    {
        transform.localScale = new Vector3(1, assignedDuration * 4, 1);
    }
}