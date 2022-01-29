using System;
using System.Collections;
using System.Collections.Generic;
using Interactor;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DeliveryTimeView : MonoBehaviour
{

    [Inject] private BasketInteractor basketInteractor;
    

    void Awake()
    {
        basketInteractor.AddDeliveryDate(DateTime.Today.ToString());
        basketInteractor.AddDeliveryTime("11.00 -15.00");
    }
}
