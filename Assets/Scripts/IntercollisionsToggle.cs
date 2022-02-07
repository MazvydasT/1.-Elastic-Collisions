using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class IntercollisionsToggle : MonoBehaviour
{
    Toggle toggle;

    [SerializeField]
    CircleManager circleManager;

    // Start is called before the first frame update
    void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(v => circleManager.Intercollisions = v);
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveAllListeners();
    }
}
