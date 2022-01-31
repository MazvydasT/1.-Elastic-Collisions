using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CircleCounter : MonoBehaviour
{
    [SerializeField]
    CircleManager circleManager;

    TextMeshProUGUI textMesh;

    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        textMesh.text = $"Count: {circleManager.CircleCount:n0}";
    }
}
