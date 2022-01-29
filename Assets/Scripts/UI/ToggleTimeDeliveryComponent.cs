using System;
using System.Collections;
using Interactor;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ToggleTimeDeliveryComponent : MonoBehaviour
{
    [Inject] private BasketInteractor basketInteractor;

    [SerializeField] private Color red, white, black, blue;

    [SerializeField] private Image caution, background;
    [SerializeField] private TextMeshProUGUI cautionText, timeText, price;
    private string time;
    private Toggle tgl;
    void Awake()
    {
        time = timeText.text;
        tgl = GetComponent<Toggle>();
        tgl.onValueChanged.RemoveAllListeners();
        tgl.onValueChanged.AddListener(Activate);
        Activate(tgl.isOn);
        basketInteractor.OnChangeDate.Subscribe(CheckPossibility);
        CheckPossibility(DateTime.Today);
    }

    public void CheckPossibility(DateTime date)
    {
        var t = date.AddHours(int.Parse(timeText.text.Substring(0, 2))+2)
            .Subtract(DateTime.Now);
        tgl.interactable = t.Ticks > 0;
        tgl.group.SetAllTogglesOff();
        Activate(false);
        if (!tgl.interactable) return;
        if ((t.Hours < 0 || t.Hours >= 2) && (t.Hours <= 8 || timeText.text.Substring(0, 2) != "10")) return;
        StartCoroutine(Act());
    }

    IEnumerator Act()
    {
        yield return new WaitForFixedUpdate();
        tgl.isOn = true;
        Activate(true);
    }
    public void Activate(bool v)
    {
        if (v)
        {
            caution.color = white;
            cautionText.color = white;
            background.color = blue;
            timeText.color = white;
            price.color = white;
            basketInteractor.AddDeliveryTime(time);
        }
        else
        {
            caution.color = red;
            cautionText.color = red;
            background.color = white;
            timeText.color = black;
            price.color = black;
        }
    }
}
