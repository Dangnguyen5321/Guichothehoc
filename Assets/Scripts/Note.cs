using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    double timeInstantiated;
    public float assignedTime;
    public float assignedDuration; // Thoi luong not
    void Start()
    {
        timeInstantiated = SongManager.GetAudioSourceTime();
        Debug.Log($"[Note] Spawned - Assigned Time: {assignedTime}, Assigned Duration: {assignedDuration}");
    }

    // Update is called once per frame
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
            //// Keo dai not theo dung khoang thoi gian thuc
            //float noteSpeed = Mathf.Abs(SongManager.Instance.noteSpawnY - SongManager.Instance.noteTapY) / (SongManager.Instance.noteTime); // Toc do not di chuyen
            //float length = noteSpeed * assignedDuration; // Do dai not tuong ung thoi gian
            //transform.localScale = new Vector3(transform.localScale.x, length, transform.localScale.z);
            // Nếu muốn sử dụng thời lượng để thay đổi kích thước nốt
            UpdateNoteAppearance();

            GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    private void UpdateNoteAppearance()
    {
        // Ví dụ: Thay đổi chiều dài của nốt theo thời lượng
        transform.localScale = new Vector3(1, assignedDuration * 4, 1); // Tăng chiều dài theo thời lượng
    }
}
