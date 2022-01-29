using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Entity;
using Interactor;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace Delivery.Views
{
    public class OrderInfoView : MonoBehaviour
    {
        [SerializeField] private OrderAssemblyView orderAssemblyView;
        [SerializeField] TextMeshProUGUI OrderNumber, Date, Status, Cash, DeliveryCash, DiscountCash;
        private AccountOrdersEntity order;

        [Inject] private ChangeViewInteractor changeViewInteractor;
        [Inject] private BasketInteractor basketInteractor;
        [Inject] private NotificationInteractor notificationInteractor;
        [Inject] private ShopInteractor shopInteractor;
        
        public void SetOrder(AccountOrdersEntity ordersEntity)
        {
            order = ordersEntity;
            OrderNumber.text = "№ заказа: " + order.Id;
            Date.text = order.Dt.DateTime.ToString("d MMMM HH:mm", CultureInfo.GetCultureInfo("ru-ru"));
            Status.text = "Статус: " + order.Status;
            Cash.text = float.Parse(order.CashAmount, CultureInfo.InvariantCulture).ToString("0.00") + " ₽";
            DeliveryCash.text = "Нет данных";
            DiscountCash.text = "Нет данных";
        }

        public void LookAtOrder()
        {
            orderAssemblyView.SetOrder(order);
            changeViewInteractor.ChangeView(19);
        }

        public void AddOrderToBasket()
        {
            basketInteractor.CleanBasket();
            foreach (var good in order.Goods)
            {
                var _ = new OrderSearchHandler(good, shopInteractor, basketInteractor);
            }
        }

        private class OrderSearchHandler
        {
            private AccountGoodsEntity good;
            private BasketInteractor _basketInteractor;
            public OrderSearchHandler(
                AccountGoodsEntity entity, 
                ShopInteractor shopInteractor, 
                BasketInteractor basketInteractor)
            {
                good = entity;
                _basketInteractor = basketInteractor;
                shopInteractor.SearchForGoods(good.Name, AddToBasket);
            }

            private void AddToBasket(ProductEntity[] products)
            {
                if (products == null) return;
                foreach (var product in products)
                {
                    product.OrderQuantity = float.Parse(good.Quantity, CultureInfo.InvariantCulture);
                    _basketInteractor.AddToBasket(product);
                }
            }
        }
    }
}