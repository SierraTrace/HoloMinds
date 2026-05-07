using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelfEsteemBar : MonoBehaviour
{
    public static SelfEsteemBar Instance;

    public Slider slider;
    public TextMeshProUGUI scoreText;


    private void Awake()
    {
        Instance = this;
    }

    public void SetMaxValue(float max)
    {
        slider.maxValue = max;
    }

    public void SetValue(float value)
    {
        slider.value = value;

        if (scoreText != null)
        {
            scoreText.text = Mathf.FloorToInt(value).ToString();
        }
    }
}
