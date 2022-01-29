using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.UI;

public class AlternativeToggle : MonoBehaviour
{
    private List<Toggle> toggles;
    private int current;
    void Awake()
    {
        toggles = new List<Toggle>();
        foreach (Transform child in transform) toggles.Add(child.GetComponent<Toggle>());
        current = toggles.Count / 2;
        toggles[current].isOn = true;
    }

    public void NextToggle()
    {
        current = current + 1 >= toggles.Count ? 0 : current + 1;
        toggles[current].isOn = true;
    }
    public void PreviousToggle()
    {
        current = current - 1 < 0 ? toggles.Count - 1 : current - 1;
        toggles[current].isOn = true;
    }
}
