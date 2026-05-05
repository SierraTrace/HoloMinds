using UnityEngine;
using UnityEngine.UI;

public class SelfEsteemBar : MonoBehaviour
{
    public static SelfEsteemBar Instance;

    public Slider slider;

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
    }
}
