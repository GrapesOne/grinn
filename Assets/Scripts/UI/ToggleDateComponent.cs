using System;
using System.Globalization;
using Interactor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ToggleDateComponent : MonoBehaviour
{
    [SerializeField] private Color 
        First = new Color(0.09803922f,0.1254902f,0.1568628f), 
        Second = Color.white;

    [SerializeField] private int day;
    [Inject] private BasketInteractor basketInteractor;

    private TextMeshProUGUI text;
    private GameObject WhitePart;
    void Awake()
    {
        WhitePart = transform.GetChild(0).gameObject;
        text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = day switch
        {
            0 => "Сегодня",
            1 => "Завтра, "+(DateTime.Today+ TimeSpan.FromDays(day))
            .ToString("d MMMM ", CultureInfo.GetCultureInfo("ru-ru")),
            _ => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                (DateTime.Today + TimeSpan.FromDays(day)).
                    ToString("dddd, d MMMM ", CultureInfo.GetCultureInfo("ru-ru")))
        };
        ChangeColor(GetComponent<Toggle>().isOn);
    }


    public void ChangeColor(bool v)
    {
        if (v)
        {
            text.color = Second;
            WhitePart.SetActive(false);
            basketInteractor.AddDeliveryDate(DateTime.Today.AddDays(day).ToString());
        }
        else
        {
            text.color = First;
            WhitePart.SetActive(true);
        }
        
    }
}
