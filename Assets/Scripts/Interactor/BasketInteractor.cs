using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Entity;
using Gateway;
using Sirenix.Utilities;
using UniRx;
using Zenject;

namespace Interactor
{
    public class  BasketInteractor : BaseBasketInteractor
    {
        
        [Inject] private ShopGateway shopGateway;
        [Inject] private AccountInteractor accountInteractor;
        [Inject] private NotificationInteractor notificationInteractor;
        [Inject] private ChangeViewInteractor changeViewInteractor;
        [Inject] private ShopLocationInteractor shopLocationInteractor;

        private ISubject<(bool, float,string)> onChangeDeliveryType = new Subject<(bool, float,string)>();
        public IObservable<(bool, float,string)> OnChangeDeliveryType => onChangeDeliveryType;
        
        private ISubject<(bool, string, PaymentType)> onChangePaymentType = new Subject<(bool,string, PaymentType)>();
        public IObservable<(bool,string, PaymentType)> OnChangePaymentType => onChangePaymentType;
        
        private ISubject<DateTime> onChangeDate = new Subject<DateTime>();
        public IObservable<DateTime> OnChangeDate => onChangeDate;
        
        private ISubject<OrderEntity> onCreateOrder = new Subject<OrderEntity>();
        public IObservable<OrderEntity> OnCreateOrder => onCreateOrder;
        
        private float weight, discount, totalPrice;
        private Basket _basket = new Basket();
        private KeyValuePair<Id, ProductEntity> cachedProduct = new KeyValuePair<Id, ProductEntity>();

        public void SetProductToCache(Id id, ProductEntity val) 
            => cachedProduct = new KeyValuePair<Id, ProductEntity>(id, val);

        public void SetProductToCache(ProductEntity val)
        {
            var id = new Id {Value = $"{count++:00000}"};
            cachedProduct = new KeyValuePair<Id, ProductEntity>(id, val);
        }

        public void SetAddress(string text) => _basket.orderAddress = text;
        public string GetAddress() => _basket.orderAddress;

        public KeyValuePair<Id, ProductEntity> GetCachedProduct() => cachedProduct;

        public void CreateBasket()
        {
            _basket = new Basket
            {
                items = basketProducts.Select(product => product.Value)
                    .Select(val => new Item
                    {
                        id = val.Id,
                        name = val.Name,
                        image = val.Image,
                        measure = val.Measure,
                        quantity = val.OrderQuantity,
                        max = val.Max,
                        min = val.Min,
                        step = val.Step,
                        regprice = val.RegPrice,
                        actprice = val.ActPrice,
                        pickup = val.Pickup,
                        fimage = val.Fimage,
                        price = val.ActPrice,
                        amount = float.Parse(val.ActPrice, CultureInfo.InvariantCulture) * val.OrderQuantity
                    })
                    .ToArray(),
                amount = basketProducts.Count,
                orderCashAmount = totalPrice,
                orderDeliveryDate = _basket.orderDeliveryDate,
                orderDeliveryTime = _basket.orderDeliveryTime,
                delivery = _basket.delivery,
                total = _basket.delivery + totalPrice,
                orderAddress = _basket.orderAddress,
                deliveryAvail = _basket.deliveryAvail
            };
        }

        public (int,float, float, float, float) GetCharacteristics()
        {
            FindCharacteristics();
            return (basketProducts.Count, _basket.delivery, weight, discount, totalPrice);
        }
        public (string, string) GetTime() => (_basket.orderDeliveryTime, _basket.orderDeliveryDate);

        public void SetDelivery((bool deliveryAvail, float delivery, string orderAddress ) val)
        {
            _basket.deliveryAvail = val.deliveryAvail;
            _basket.delivery = val.delivery;
            _basket.orderAddress = val.orderAddress;
            onChangeDeliveryType.OnNext(val);
        }
        public void SetPayment(bool hasCard, string cardNumber, PaymentType orderCashType)
        {
            _basket.hasCard = hasCard;
            _basket.orderCashType = orderCashType.ToString();
            onChangePaymentType.OnNext((hasCard,cardNumber, orderCashType));
        }
        void FindCharacteristics()
        {
            var hasCard = accountInteractor.HasCard();
            SetToNull();
            foreach (var product in basketProducts) Routine(product.Value, hasCard);
        }

        public void AddDeliveryDate(string date)
        {
            onChangeDate.OnNext(DateTime.Parse(date));
            _basket.orderDeliveryDate = date;
        }

        public void AddDeliveryTime(string time) => _basket.orderDeliveryTime = time;
        
        void SetToNull()
        {
            weight = .0f;
            discount = .0f;
            totalPrice = .0f;
        }

        void Routine(ProductEntity product, bool hasCard)
        {
            switch (product.Measure)
            {
                case "KG":
                case "кг":
                    weight += product.OrderQuantity;
                    break;
                case "г":
                case "G":
                    weight += product.OrderQuantity/1000f;
                    break;
            }
            var tempPrice = product.OrderQuantity * float.Parse(product.RegPrice, CultureInfo.InvariantCulture);
            discount += tempPrice;
            var tempPrice2 = product.OrderQuantity *  float.Parse(product.ActPrice, CultureInfo.InvariantCulture);
            discount -= tempPrice2;
            totalPrice += hasCard ? tempPrice2: tempPrice;
        }
        
            
        
        
        
        
        public OrderEntity CreateOrder()
        {
            if (_basket.orderAddress.IsNullOrWhitespace())
            {
                notificationInteractor.ShowNotification(Reply.CannotCreateOrder, Reply.NoAddress);
                return null;
            }
            if (basketProducts.Count == 0)
            {
                notificationInteractor.ShowNotification(Reply.CannotCreateOrder, Reply.NoProducts);
                return null;
            }
            accountInteractor.TryGetAccountEntity(out var account);
            if (account == null)
            {
                notificationInteractor.ShowNotification(Reply.CannotCreateOrder, Reply.NotAuthorized);
                changeViewInteractor.ChangeView(6);
                return null;
            }
            
            CreateBasket();
            account.ShopId = shopLocationInteractor.ShopId();
            shopGateway.Order(account, _basket,OnSuccess,OnError);
            CleanBasket();
            return shopGateway.GetLastOrder();
        }
            

      
    }

    
}
