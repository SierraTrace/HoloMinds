using UnityEngine;
using TMPro;

public class FinalScoreUI : MonoBehaviour
{
    public TextMeshProUGUI finalScoreText;

    void Start()
    {
        finalScoreText.text = "Puntuación total: " + GameManager.Instance.totalScore;
    }

    public void BackToMainMenu()
    {
        SceneLoader.LoadSceneByName("MainMenu");
    }
}
