using UnityEngine;
using UnityEngine.UI;

public class PlayOrStopMidi : MonoBehaviour
{
    public float speed = 5f;
    private bool isMoving = false;

    public Button PlayOrStop;
    public Text buttonText;

    public ScrollCamera scrollCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            transform.position += Vector3.up * speed * Time.deltaTime;
        }
    }

    public void ToggleMovement()
    {
        isMoving = !isMoving; // dao trang thai

        if (isMoving)
        {
            buttonText.text = "Stop"; // doi text thanh Stop
            if (scrollCamera != null) scrollCamera.enabled = false;
        }
        else
        {
            buttonText.text = "Play"; // doi text thanh Play
            if (scrollCamera != null) 
            {
                scrollCamera.enabled = true;
                scrollCamera.ResetDragging();
            } 
        }
    }
}
