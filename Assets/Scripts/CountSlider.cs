using UnityEngine;
using UnityEngine.UI;

public class CountSlider : MonoBehaviour
{
    Slider slider;

    [SerializeField]
    CircleManager circleManager;

    void Start()
    {
        slider = GetComponent<Slider>();

        circleManager.CircleCount = (int)Mathf.Clamp(circleManager.CircleCount, slider.minValue, slider.maxValue);

        slider.value = circleManager.CircleCount;

        slider.onValueChanged.AddListener(v => circleManager.CircleCount = (int)v);
    }
}
