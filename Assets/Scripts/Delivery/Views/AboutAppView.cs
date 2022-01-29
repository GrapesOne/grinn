using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AboutAppView : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI versionText;
    void Awake() => versionText.text = "Установленная версия: "+Application.version;
}
