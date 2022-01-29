using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class ScrollToTop : MonoBehaviour
{
    private ScrollRect ScrollRect;
    void Awake() => ScrollRect = GetComponent<ScrollRect>();
    void OnEnable() => ToTop();
    [Button] void ToTop() => ScrollRect.verticalNormalizedPosition = 1;
}
