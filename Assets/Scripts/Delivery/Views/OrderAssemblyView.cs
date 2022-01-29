using System.Collections;
using System.Collections.Generic;
using Entity;
using Interactor;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using Zenject;

namespace Delivery.Views
{
    public class OrderAssemblyView : MonoBehaviour
    {
        private AccountOrdersEntity order;
        [SerializeField] private GameObject ProductPrefab;
        [SerializeField] private Transform all, collected, replaced , canceled;
        [SerializeField] private TextMeshProUGUI Price;
        
        [Inject] private DownloadInteractor downloadInteractor;
        public void SetOrder(AccountOrdersEntity ordersEntity)
        {
            order = ordersEntity;
            Price.text = order.CashAmount+ " ₽";
            CreateIn(all);
            CreateIn(collected);
        }
        
        private void CreateIn(Transform parent)
        {
            foreach (Transform child in parent) Destroy(child.gameObject);
            
            foreach (var item in order.Goods)
            {
                var temp = Instantiate(ProductPrefab, parent, false);
                var pp = temp.GetComponent<ProductInOrderPrefab>();
                pp.SetupProduct(item);
                downloadInteractor.GetImage("https://linia-market.ru" + item.Image, pp.SetImage);
            }
        }
    }
}
