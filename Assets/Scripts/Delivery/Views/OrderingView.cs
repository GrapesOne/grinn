using System;
using Gateway;
using Interactor;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using Zenject;

public class OrderingView : MonoBehaviour
{
    [SerializeField] private GameObject Where, Whence;
    [SerializeField] private TextMeshProUGUI Count, DeliveryCash, Weight, Discount, Total, DeliveryTime, WhereText, WhenceText;
    [Inject] private BasketInteractor basketInteractor;
    [Inject] private OrderInteractor orderInteractor;
    [Inject] private ChangeViewInteractor changeViewInteractor;
    [Inject] private ShopLocationInteractor shopLocationInteractor;
    
    private (int, float, float, float, float) characteristics;
    private (string, string) TimeDate;

    private int[] posibleTime = {10, 12, 14, 16, 18};
    void OnEnable()
    {
        characteristics = basketInteractor.GetCharacteristics();
        Count.text = characteristics.Item1.ToString();
        Weight.text = characteristics.Item3.ToString("0.00") + " кг";
        Discount.text = characteristics.Item4.ToString("0.00")+ " ₽";
        Total.text = (characteristics.Item5 + characteristics.Item2).ToString("0.00") + " ₽";
        DeliveryCash.text = characteristics.Item2 == 0 ? "Бесплатно" 
            : characteristics.Item2.ToString("0.00") + " ₽";
        TimeDate = basketInteractor.GetTime();
        if(TimeDate.Item1.IsNullOrWhitespace() || TimeDate.Item2.IsNullOrWhitespace())
        {
            SetRightTime();
            TimeDate = basketInteractor.GetTime();
        }
        DeliveryTime.text = "\n" + ReadDate();
        WhereText.text = basketInteractor.GetAddress();
        WhenceText.text = basketInteractor.GetAddress();
    }

    void SetRightTime()
    {
        foreach (var i in posibleTime)
        {
            if (DateTime.Today.AddHours(i).Subtract(DateTime.Now).Ticks <= 0) continue;
            basketInteractor.AddDeliveryDate(DateTime.Today.ToString());
            basketInteractor.AddDeliveryTime(i+":00-"+(i+2)+":00");
            return;
        }
        basketInteractor.AddDeliveryDate(DateTime.Today.AddDays(1).ToString());
        basketInteractor.AddDeliveryTime("10:00-12:00");
    }

    string ReadDate()
    {
        var time = TimeDate.Item1.Substring(TimeDate.Item1.IndexOf("-" , 
            StringComparison.Ordinal) + 1);
        var date = DateTime.Parse(TimeDate.Item2);
        var timeSpan = TimeSpan.Parse(time);
        var dif = date.Add(timeSpan).Subtract(DateTime.Now);
        string deliveryTimeText;
        if (dif.Ticks > 0)
        {
            var h = dif.Days * 24 + dif.Hours;
            var m = dif.Minutes;
            deliveryTimeText = h == 0
                ? ""
                : (h % 10) switch
                {
                    1 => h + " час ",
                    var x when x > 1 && x < 5 => h + " часа ",
                    _ => h + " часов "
                };
            deliveryTimeText += m == 0
                ? ""
                : (m % 10) switch
                {
                    1 => m + " минуту ",
                    var x when x > 1 && x < 5 => m + " минуты ",
                    _ => m + " минут "
                };
        }
        else
        {
            SetRightTime();
            TimeDate = basketInteractor.GetTime();
            deliveryTimeText = ReadDate();
        }

        return deliveryTimeText;
    }

    public void SendOrder()
    {
        var order = basketInteractor.CreateOrder();
        if (order == null) return;
        orderInteractor.SetOrder(order);
        changeViewInteractor.ChangeView(22);
    }

    public void ChangeDeliveryTypeToDelivery(bool val)
    {
        characteristics = basketInteractor.GetCharacteristics();
        Where.SetActive(val);
        if (val)
        {
            
            TimeDate = basketInteractor.GetTime();
            if(TimeDate.Item1.IsNullOrWhitespace() || TimeDate.Item2.IsNullOrWhitespace())
            {
                SetRightTime();
                TimeDate = basketInteractor.GetTime();
            }
            DeliveryTime.text = "\n" + ReadDate();
            basketInteractor.SetDelivery((true, 99, ""));
            Total.text = (characteristics.Item5 + 99).ToString("0.00") + " ₽";
            DeliveryCash.text = 99.ToString("0.00") + " ₽";
        }
        else
        {
            Total.text = characteristics.Item5.ToString("0.00") + " ₽";
            DeliveryCash.text = "Бесплатно" ;
        }
        WhereText.text = basketInteractor.GetAddress();
    }
    public void ChangeDeliveryTypeToPickup(bool val)
    {
        Whence.SetActive(val);
        if(val) basketInteractor.SetDelivery((false, 0, shopLocationInteractor.ShopName()));
        WhenceText.text = basketInteractor.GetAddress();
    }
    
    
}
