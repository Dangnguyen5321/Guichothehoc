using UnityEngine;
using UnityEngine.UI;

public class MidiTimeLine : MonoBehaviour
{
    public Lane laneReference; // Tham chi?u ??n Lane ?? l?y zoomFactor và n?t
    //public float width = 5f; // Chi?u r?ng c?a timeline (?? kh?p v?i giao di?n)
    private Renderer renderer; // ?? thay ??i màu s?c (hi?u ?ng tr?c quan)
    public SongManager songManager;
    public Text playPauseButtonText;
    public static MidiTimeLine Instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
        if (laneReference == null && SongManager.Instance.lanes.Length > 0)
        {
            laneReference = SongManager.Instance.lanes[0];
        }

        // ?i?u ch?nh kích th??c c?a timeline
        //transform.localScale = new Vector3(width, 0.1f, 1f); // Chi?u cao m?ng, chi?u r?ng tùy ch?nh
        transform.localPosition = new Vector3(-1.030029f, SongManager.Instance.noteTapY, 0); // B?t ??u t? noteTapY

        renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            renderer = gameObject.AddComponent<MeshRenderer>();
            gameObject.AddComponent<MeshFilter>().mesh = Resources.GetBuiltinResource<Mesh>("Cube.mesh");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!SongManager.Instance.isPlaying || laneReference == null || SongManager.midiFile == null) return;

        // L?y th?i gian hi?n t?i
        double currentTime = SongManager.GetAudioSourceTime();

        // Tính v? trí Y c?a timeline
        float yPosition = SongManager.Instance.noteTapY + ((float)currentTime * SongManager.Instance.noteTime * laneReference.zoomFactor);
        transform.localPosition = new Vector3(-1.030029f, yPosition, 0);
    }

    public void UpdateTimeLinePosition(double newTime)
    {
        float yPosition = SongManager.Instance.noteTapY + ((float)newTime * SongManager.Instance.noteTime * laneReference.zoomFactor);
        transform.localPosition = new Vector3(-1.030029f, yPosition, 0);
    }

    public void PlayPauseButtonClicked()
    {
        if (songManager != null)
        {
            songManager.PlayPauseSong();

            if (songManager.isPlaying)
            {
                playPauseButtonText.text = "Pause";
            }
            else
            {
                playPauseButtonText.text = "Play";
            }
        }
        else
        {
            Debug.LogError("SongManager ch?a ???c gán!");
        }
    }

    public void StopButtonClicked()
    {
        if (songManager != null)
        {
            songManager.StopSong();
            playPauseButtonText.text = "Play";
        }
        else
        {
            Debug.LogError("SongManager ch?a ???c gán!");
        }
    }

    public void ResetTimeline()
    {
        transform.localPosition = new Vector3(-1.030029f, SongManager.Instance.noteTapY, 0);
    }

    // Reset lai thanh nut play khi open file moi cho SongManager.cs
    public void ResetAfterOpenFile()
    {
        if (playPauseButtonText != null)
        {
            playPauseButtonText.text = "Play";
        }
    }

    public void FastForwardButtonClicked()
    {
        if (songManager != null)
        {
            songManager.FastForward();
        }
    }

    public void RewindButtonClicked()
    {
        if (songManager != null)
        {
            songManager.Rewind();
        }
    }
}
