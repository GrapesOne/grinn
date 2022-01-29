using System;
using System.Collections.Generic;
using System.Text;
using BestHTTP;
using Entity;
using Interactor;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

namespace Gateway
{
    public class ShopGateway
    {
        private static string ApiUrl = "https://linia-market.ru/api/";
        private OrderEntity LastOrder;
        [Inject] private NotificationInteractor notificationInteractor;
        public void Catalog(Action<CatalogEntity> onSuccess, Action<string> onError)
        {
            Debug.LogWarning("--- Getting Catalog ---");
            var u = new Uri(ApiUrl + "catalog.php");
            
            var request = new HTTPRequest(u, HTTPMethods.Get,
                (req, res) =>
                {
                    if (res == null)
                    {
                        Debug.LogError("Connection error");
                        notificationInteractor.NoInternetNotification();
                        return;
                    }
                    
                    if (res.DataAsText.Contains("error"))
                    {
                        onError(res.DataAsText);
                        return;
                    }
                    var categories = JsonConvert.DeserializeObject<CategoryEntity[]>(res.DataAsText);
                    var catalog = new CatalogEntity {Categories = categories};
                    onSuccess(catalog);
                });
            
            request.Send();
        }
        
        public void Goods(int shopId, int catalogId, Action<ProductEntity[]> onSuccess, Action<string> onError)
        {
            Debug.LogWarning("--- Getting Goods ---");
            
            var u = new Uri(ApiUrl + "goods.php?id_catalog=" + catalogId + "&id_shop=" + shopId);
            
            var request = new HTTPRequest(u, HTTPMethods.Get,
                (req, res) =>
                {
                    if (res == null)
                    {
                        notificationInteractor.NoInternetNotification();
                        onError("Connection error");
                        return;
                    }
                    
                    if (res.DataAsText.Contains("error"))
                    {
                        onError(res.DataAsText);
                        return;
                    }
                    
                    var products = JsonConvert.DeserializeObject<ProductEntity[]>(res.DataAsText);
                    onSuccess(products);
                });
            
            request.AddHeader("Content-Type", "application/json");
            request.Send();
        }
        
        public void Search(int shopId, string searchString, Action<ProductEntity[]> onSuccess, Action<string> onError)
        {
            Debug.LogWarning("--- Getting Goods ---");
            
            var u = new Uri(ApiUrl + "goods.php?id_shop=" + shopId + "&search=" + searchString);
            
            var request = new HTTPRequest(u, HTTPMethods.Get,
                (req, res) =>
                {
                    if (res == null)
                    {
                        notificationInteractor.NoInternetNotification();
                        onError("Connection error");
                        return;
                    }
                    
                    if (res.DataAsText.Contains("error"))
                    {
                        onError(res.DataAsText);
                        return;
                    }
                    
                    var products = JsonConvert.DeserializeObject<ProductEntity[]>(res.DataAsText);
                    onSuccess(products);
                });
            
            request.AddHeader("Content-Type", "application/json");
            request.Send();
        }
        
        public void Order(AccountEntity account, ProductEntity[] products, Action<string> onSuccess = null, Action<string> onError= null)
        {
            Debug.LogWarning("--- Ordering ---");
            
            var u = new Uri(ApiUrl + "order.php");
            
            var request = new HTTPRequest(u, HTTPMethods.Post,
                (req, res) =>
                {
                    if (res == null)
                    {
                        notificationInteractor.NoInternetNotification();
                        onError?.Invoke("Connection error");
                        return;
                    }
            
                    var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(res.DataAsText);
                    
                    if (response.ContainsKey("error"))
                    {
                        onError?.Invoke(response["error"]);
                        return;
                    }
                    
                    if (response.ContainsKey("id_order"))
                    {
                        var result = response["id_order"];
                        onSuccess?.Invoke(result);
                    } 
                });
            
            request.AddHeader("Content-Type", "application/json");
            var data = CreateOrderDataString(account, products);
            request.RawData = Encoding.UTF8.GetBytes(data);
            request.Send();
        }
        

        private string CreateOrderDataString(AccountEntity account, ProductEntity[] products)
        {
            var order = GenerateOrder(account, products);
            var orderAsText = JsonConvert.SerializeObject(order);
            Debug.Log("Example order: " + orderAsText);
            return orderAsText;
        }
        
        private OrderEntity GenerateOrder(AccountEntity account, IReadOnlyList<ProductEntity> products)
        {
            var order = new OrderEntity
            {
                id = account.Id,
                sessid = account.SessId,
                id_shop = account.ShopId,
                basket = new Basket
                {
                    items = new Item[products.Count],
                    //TODO get all this info from?
                    orderType = "???",
                    orderAddress = "???",
                    orderCashType = "???",
                    orderCashAmount = 1,
                    orderDeliveryDate = "???",
                    orderDeliveryTime = "???",
                    orderComment = "???",
                    hasCard = true,
                    amount = 1,
                    deliveryAvail = true,
                    delivery = 1,
                    total = 1,
                    orderAvail = true,
                    cashAvail = true
                }
            };

            for(var i = 0; i < products.Count; i++)
                order.basket.items[i] = new Item
                {
                    id = products[i].Id,
                    name = products[i].Name,
                    image = products[i].Image,
                    measure = products[i].Measure,
                    quantity = products[i].OrderQuantity,
                    max = products[i].Max,
                    min = products[i].Min,
                    step = products[i].Step,
                    regprice = products[i].RegPrice,
                    actprice = products[i].ActPrice,
                    pickup = products[i].Pickup,
                    fimage = products[i].Fimage,
                    price = "9.99", //TODO which price is this (price * amount???)
                    amount = 5
                };

            return order;
        }
        
        private void PrintResponseKeys(Dictionary<string, string> response)
        {
            foreach(var pair in response)
                Debug.Log("Response. Key: " + pair.Key + ", Value: " + pair.Value);
        }
        public void Order(AccountEntity account, Basket basket, Action<string> onSuccess = null, Action<string> onError= null)
        {
            Debug.LogWarning("--- Ordering ---");
            
            var u = new Uri(ApiUrl + "order.php");
            
            var request = new HTTPRequest(u, HTTPMethods.Post,
                (req, res) =>
                {
                    if (res == null)
                    {
                        notificationInteractor.NoInternetNotification();
                        onError?.Invoke("Connection error");
                        return;
                    }
            
                    var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(res.DataAsText);
                    
                    if (response.ContainsKey("error"))
                    {
                        onError?.Invoke(response["error"]);
                        return;
                    }
                    
                    if (response.ContainsKey("id_order"))
                    {
                        var result = response["id_order"];
                        onSuccess?.Invoke(result);
                    } 
                });
            
            request.AddHeader("Content-Type", "application/json");
            var data = CreateOrderDataString(account, basket);
            request.RawData = Encoding.UTF8.GetBytes(data);
            request.Send();
        }
         private string CreateOrderDataString(AccountEntity account, Basket basket)
        {
            var order = GenerateOrder(account, basket);
            LastOrder = order;
            var orderAsText = JsonConvert.SerializeObject(order);
            Debug.Log("Example order: " + orderAsText);
            return orderAsText;
        }
        
        private OrderEntity GenerateOrder(AccountEntity account, Basket _basket) =>
            new OrderEntity
            {
                id = account.Id,
                sessid = account.SessId,
                id_shop = account.ShopId,
                basket = _basket,
            };

        [CanBeNull] public OrderEntity GetLastOrder() => LastOrder;
    }
}