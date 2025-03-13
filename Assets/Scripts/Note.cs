using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    //double timeInstantiated;
    public float assignedTime;
    public float assignedDuration; // Thoi luong not
    public float Zoom = 1f; // Giá tr? m?c ??nh là 1
    public static float MinZoom = 1f; // Gi?i h?n nh? nh?t
    public static float MaxZoom = 15f; // Gi?i h?n l?n nh?t

    void Start()
    {
        //timeInstantiated = SongManager.GetAudioSourceTime();
        //Debug.Log($"[Note] Spawned - Assigned Time: {assignedTime}, Assigned Duration: {assignedDuration}");
        // ?i?u ch?nh v? trí kh?i t?o theo th?i l??ng c?a n?t
    }

    // Update is called once per frame
    void Update()
    {
        //double timeSinceInstantiated = SongManager.GetAudioSourceTime() - timeInstantiated;
        //float t = (float)(timeSinceInstantiated / (SongManager.Instance.noteTime * 2));


        //if (t > 1)
        //{
        //    Destroy(gameObject);
        //}
        //else
        //{
        //    transform.localPosition = Vector3.Lerp(Vector3.up * SongManager.Instance.noteSpawnY, Vector3.up * SongManager.Instance.noteDespawnY, t);
        //    UpdateNoteAppearance();

        //    GetComponent<MeshRenderer>().enabled = true;
        //}
        UpdateNoteAppearance();
        GetComponent<MeshRenderer>().enabled = true;
        HandleZoomInput();
    }

    private void UpdateNoteAppearance()
    {
        transform.localScale = new Vector2(1, assignedDuration * Zoom);
    }

    public void HandleZoomInput()
    {
        if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus)) // Phím '=' trên bàn phím th??ng ho?c '+' trên numpad
        {
            Zoom = Mathf.Min(Zoom + 1f, MaxZoom);
        }
        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus)) // Phím '-' trên bàn phím th??ng ho?c numpad
        {
            Zoom = Mathf.Max(Zoom - 1f, MinZoom);
        }
    }
}
