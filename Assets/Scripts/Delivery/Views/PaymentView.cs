using System.Collections;
using System.Collections.Generic;
using Interactor;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PaymentView : MonoBehaviour
{
    [SerializeField] private ToggleGroup _group;
    [Inject] private BasketInteractor _basketInteractor;

    public void Change(bool val)
    {
        if(!val) return;
        Debug.Log(_group.AnyTogglesOn());
        Debug.Log(_group.ActiveToggles());
        Debug.Log(_group.GetFirstActiveToggle().GetComponent<TogglePayment>());
        var paymentInfo = _group.GetFirstActiveToggle().GetComponent<TogglePayment>();
        
        _basketInteractor.SetPayment(paymentInfo.isCard, paymentInfo.CardNum, paymentInfo.PaymentType);
    }

}
public enum PaymentType
{
    Card, Cash, CourierByCard, GooglePay
}
