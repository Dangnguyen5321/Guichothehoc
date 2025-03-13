using UnityEngine;

public class ScrollCamera : MonoBehaviour
{
    public float scrollSpeed = 0.1f; // Tốc độ cuộn camera
    private bool isDragging = false;
    private Vector3 lastMousePosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Khi bấm chuột trái
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0)) // Khi thả chuột trái
        {
            isDragging = false;
        }

        if (isDragging) // Khi đang giữ chuột trái
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            Vector3 position = transform.position;
            position.y -= delta.y * scrollSpeed * Time.deltaTime; // Kéo lên thì camera xuống, kéo xuống thì camera lên

            // Cập nhật vị trí camera
            transform.position = position;

            lastMousePosition = Input.mousePosition;
        }
    }
    public void ResetDragging()
    {
        isDragging = false;
    }
}
