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

    public Color gizmoColor = Color.red;
    public float gizmoRadius = 0.5f;

    // Hàm này được gọi trong editor để vẽ gizmo
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor; // Thiết lập màu cho gizmo

        // Vẽ một vòng tròn 2D tại vị trí của GameObject
        Gizmos.DrawWireSphere(transform.position, gizmoRadius);
    }

    // Hàm này chỉ được gọi khi GameObject được chọn trong editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green; // Màu khác khi được chọn

        // Vẽ một vòng tròn 2D tại vị trí của GameObject khi được chọn
        Gizmos.DrawWireSphere(transform.position, gizmoRadius);
    }
}