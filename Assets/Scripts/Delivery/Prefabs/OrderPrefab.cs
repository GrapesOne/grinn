using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Delivery.Views;
using Entity;
using Interactor;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class OrderPrefab : MonoBehaviour
{
    public TextMeshProUGUI OrderNumber, Date, Status, Cash;
    private AccountOrdersEntity ordersEntity;
    private UserOrdersView view;
    public void SetOrder(AccountOrdersEntity order, UserOrdersView ordersView)
    {
        view = ordersView;
        ordersEntity = order;
        OrderNumber.text = "№ заказа: " + order.Id;
        Date.text = order.Dt.DateTime.ToString("d MMMM HH:mm", CultureInfo.GetCultureInfo("ru-ru"));
        Status.text = "Статус: " + order.Status;
        Debug.Log(order.CashAmount);
        Cash.text = float.Parse(order.CashAmount, CultureInfo.InvariantCulture).ToString("0.00") + " ₽";
    }

    public void Choose()
    {
        view.SetOrder(ordersEntity);
    } 
}
