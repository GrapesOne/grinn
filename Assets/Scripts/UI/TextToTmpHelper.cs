using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextToTmpHelper : MonoBehaviour
{
    [Button]
    void Change()
    {
        var txt = GetComponent<Text>();
        var size = txt.fontSize;
        var text = txt.text;
        var col = txt.color;
        DestroyImmediate(txt);
        var tmp = gameObject.AddComponent<TextMeshProUGUI>();
        tmp.fontSize = size;
        tmp.text = text;
        tmp.color = col;
    }

    
}
