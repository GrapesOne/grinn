using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class TwoTMPInOneBoxHelper : MonoBehaviour
{
    public float CharSize, Gap;
    public bool LeftImportant;
    //public TextInfo Left, Right;
    [Button]
    void Invoke()
    {
        var rect = GetComponent<RectTransform>();
        var leftTmp = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        var rightTmp = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        if (LeftImportant) Routine(leftTmp, rightTmp, rect);
        else Routine(rightTmp, leftTmp, rect);
    }

    void Routine(TextMeshProUGUI a, TextMeshProUGUI b, RectTransform c)
    {
        var width = a.text.Length * CharSize;
        a.rectTransform.sizeDelta = new Vector2(width, a.rectTransform.sizeDelta.y);
        b.rectTransform.sizeDelta = new Vector2(c.rect.width-width-Gap, 
            b.rectTransform.sizeDelta.y);
        a.overflowMode = TextOverflowModes.Overflow;
        b.overflowMode = TextOverflowModes.Ellipsis;
        a.enableWordWrapping = false;
        b.enableWordWrapping = false;
    }
    
    /*[Serializable]
    public struct TextInfo
    {
        public TextAlignmentOptions AlignmentOptions;
        public string Text;
        public Color Color;
        public float FontSize;
    }*/
}
