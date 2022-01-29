using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using UnityEngine;

public class TogglePayment : MonoBehaviour
{
    public bool isCard;
    [ShowIf(nameof(isCard))]public string CardNum;
    [Space]
    public PaymentType PaymentType;
   
}

