using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class CameraSlider : MonoBehaviour
{
    Slider slider;

    [SerializeField]
    Transform cameraTransform;

    void Start()
    {
        slider = GetComponent<Slider>();

        cameraTransform.position = new Vector3(0, 0, Mathf.Clamp(cameraTransform.position.z, slider.minValue, slider.maxValue));

        slider.value = cameraTransform.position.z;

        slider.onValueChanged.AddListener(v => cameraTransform.position = new Vector3(0, 0, v));
    }

    private void OnDestroy()
    {
        slider.onValueChanged.RemoveAllListeners();
    }
}
