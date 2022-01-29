using System;
using System.Globalization;
using Interactor;
using TMPro;
using UnityEngine;
using Zenject;

public class CheckoutView : MonoBehaviour
{
    [Inject] private OrderInteractor orderInteractor;
    [Inject] private AccountInteractor accountInteractor;

    [SerializeField] private TextMeshProUGUI Date, Name, Id, Phone, Weight, Count, Addres, TotalPrice;
    // Start is called before the first frame update
    void OnEnable()
    {
        var order = orderInteractor.GetOrder();
        var date = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
            DateTime.Parse(order.basket.orderDeliveryDate)
                .ToString("dddd, d MMMM ", CultureInfo.GetCultureInfo("ru-ru")));
        Date.text = order.basket.deliveryAvail ? ("Доставка" + "\n" + date + order.basket.orderDeliveryTime)
                : "Самовывоз";
        Id.text = order.id;
        var weight = 0.0f;
        foreach (var item in order.basket.items)
        {
            switch (item.measure)
            {
                case "KG":
                case "кг":
                    weight += item.quantity;
                    break;
                case "г":
                case "G":
                    weight += item.quantity/1000f;
                    break;
            }
        }

        Weight.text = weight.ToString("0.00") + " кг";
        Count.text = order.basket.amount.ToString();
        Addres.text = order.basket.orderAddress;
        if (!accountInteractor.TryGetAccountEntity(out var account)) return;
        Name.text =  account.Name;
        Phone.text = account.Phone;
        TotalPrice.text = order.basket.total.ToString("0.00") + " ₽";
    }

    
}
