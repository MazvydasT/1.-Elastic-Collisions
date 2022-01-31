using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FPS : MonoBehaviour
{
    TextMeshProUGUI textMesh;

    float timeAccumulator = 0;
    int counter = 0;

    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (timeAccumulator >= 1f)
        {
            textMesh.text = $"FPS: {(1f / (timeAccumulator / counter)):n0}";

            timeAccumulator = 0;
            counter = 0;
        }

        timeAccumulator += Time.deltaTime;
        ++counter;
    }
}
