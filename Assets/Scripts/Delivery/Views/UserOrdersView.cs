using System.Collections;
using System.Collections.Generic;
using Entity;
using Interactor;
using UnityEngine;
using Zenject;

namespace Delivery.Views
{
    public class UserOrdersView : MonoBehaviour
    {
        [Inject] private AccountInteractor accountInteractor;
        [Inject] private ChangeViewInteractor changeViewInteractor;
        [SerializeField] private GameObject prefab;
        [SerializeField] private GameObject content;
        [SerializeField] private OrderInfoView InfoView;

        
        void OnEnable()
        {
            if (!accountInteractor.TryGetAccountEntity(out var account)) return;
            var orders = account.Orders;
            DestroyOldProductList();
            foreach (var order in orders)
            {
                var go = Instantiate(prefab, content.transform);
                var orderInfo = go.GetComponent<OrderPrefab>();
                orderInfo.SetOrder(order, this);
            }
        }
        private void DestroyOldProductList()
        {
            foreach (Transform child in content.transform) Destroy(child.gameObject);
        }

        public void SetOrder(AccountOrdersEntity order)
        {
            InfoView.SetOrder(order);
            changeViewInteractor.ChangeView(18);
        }
    }
}