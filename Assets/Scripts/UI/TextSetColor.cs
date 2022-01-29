using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class TextSetColor : MonoBehaviour
{
    [SerializeField] private Color ActiveColor, DeactiveColor;
    private TextMeshProUGUI text;
    void OnValidate() => text = GetComponent<TextMeshProUGUI>();
    [Button] public void SetActiveColor(bool val) => text.color = val ? ActiveColor : DeactiveColor;
}
