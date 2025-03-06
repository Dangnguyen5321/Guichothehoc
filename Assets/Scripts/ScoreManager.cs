using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    // public static ScoreManager Instance;
    // public AudioSource hitSFX;
    // public AudioSource missSFX;
    // public TextMeshProUGUI scoreText; // Đảm bảo sử dụng TextMeshProUGUI thay vì TextMeshPro
    // private static int comboScore;

    // void Start()
    // {
    //     Instance = this;
    //     comboScore = 0;
    //     UpdateScoreText(); // Cập nhật điểm số ban đầu
    // }

    // public static void Hit()
    // {
    //     comboScore += 1; // Tăng điểm khi nhấn trúng
    //     Instance.hitSFX.Play();
    //     Instance.UpdateScoreText(); // Cập nhật UI ngay lập tức
    // }

    // public static void Miss()
    // {
    //     comboScore = 0; // Reset điểm khi miss
    //     Instance.missSFX.Play();
    //     Instance.UpdateScoreText(); // Cập nhật UI ngay lập tức
    // }

    // private void UpdateScoreText()
    // {
    //     if (scoreText != null)
    //     {
    //         scoreText.text = comboScore.ToString(); // Hiển thị điểm số
    //     }
    //     else
    //     {
    //         Debug.LogError("ScoreText chưa được gán trong Inspector!");
    //     }
    // }
}