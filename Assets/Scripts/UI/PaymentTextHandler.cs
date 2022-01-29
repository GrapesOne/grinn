using System;
using System.Collections;
using System.Collections.Generic;
using Interactor;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PaymentTextHandler : MonoBehaviour
{
    [Inject] private BasketInteractor _basketInteractor;
    [SerializeField] private GameObject Card, Other;
    [SerializeField] private Image CardImage;
    [SerializeField] private TextMeshProUGUI CardName, OtherName;
    void Awake()
    {
        _basketInteractor.OnChangePaymentType.Subscribe(ChangePaymentType);
        _basketInteractor.SetPayment(false, "", PaymentType.Cash);
    }

  
    void ChangePaymentType((bool, string, PaymentType) val)
    {
        Card.SetActive(val.Item1);
        Other.SetActive(!val.Item1);
        if (val.Item1)
        {
            CardName.text = val.Item2;
        }
        else
        {
            OtherName.text = val.Item3 switch
            {
                PaymentType.Card => "",
                PaymentType.Cash => "Наличными",
                PaymentType.CourierByCard => "Картой курьеру",
                PaymentType.GooglePay => "Google Pay",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
