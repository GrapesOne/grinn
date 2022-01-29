using System.Collections.Generic;
using System.Globalization;
using Entity;
using Interactor;
using TMPro;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Delivery.Views
{
    public class BasketView : MonoBehaviour
    {
        [SerializeField] private GameObject productPrefab, favouritePrefab;
        [SerializeField] private GameObject productParent, favouriteParent, bottom, bottomBlock, info;
        [SerializeField] private Toggle basketToggle, favouriteToggle;
        
        [SerializeField] private TextMeshProUGUI countText, weightText, deliveryText, discountText, totalPriceText;

        [Inject] private FavouritesInteractor favouritesInteractor;
        [Inject] private BasketInteractor basketInteractor;
        [Inject] private OrderInteractor orderInteractor;
        [Inject] private DownloadInteractor downloadInteractor;
        [Inject] private ChangeViewInteractor changeViewInteractor;
        [Inject] private NotificationInteractor notificationInteractor;
        
        const string title = "Внимание!",
            body = "Для продолжения оформления заказа стоимость должна быть более 1000 ₽";
        
        void Awake()
        {
            basketInteractor.OnChangeQuantity.Subscribe(UpdateBottomInfo);
        }
        void OnEnable()
        {
            basketToggle.SetIsOnWithoutNotify(true);
            favouriteToggle.SetIsOnWithoutNotify(true);
            OpenBasket(true);
            SetText(basketInteractor.GetCharacteristics());
        }

        public void OpenFavourite(bool val)
        {
            if (val)
            {
                var products = favouritesInteractor.GetBasketProducts();
                if (products.Count > 0)
                {
                    favouriteParent.SetActive(true);
                    CreateTab(products, favouritePrefab, favouriteParent.transform);
                    info.SetActive(false);
                }
                else
                {
                    favouriteParent.SetActive(false);
                    info.SetActive(true);
                }
            }
            else
            {
                favouriteParent.SetActive(false);
                info.SetActive(false);
            }
        }
        public void OpenBasket(bool val)
        {
            if (val) CreateTab(basketInteractor.GetBasketProducts(), productPrefab, productParent.transform);
            productParent.SetActive(val);
            bottom.SetActive(val);
            bottomBlock.SetActive(val);
        }
        
        void CreateTab(Dictionary<BaseBasketInteractor.Id, ProductEntity> products, GameObject prefab, Transform parent)
        {
            DestroyOldProductList(parent);
            foreach (var pair in products)
            {
                var go = Instantiate(prefab, parent);
                var pp = go.GetComponent<ProductPrefabBase>();
                pp.SetupFavouritesInteractor(favouritesInteractor);
                pp.SetupBasketInteractor(basketInteractor);
                pp.SetupProduct(pair.Value, pair.Key);
                downloadInteractor.GetImage("https://linia-market.ru" + pair.Value.Image, 
                    imageTexture => pp.SetImage(imageTexture));
            }
        }
        private void UpdateBottomInfo(ProductEntity _ = null) => SetText(basketInteractor.GetCharacteristics());
        void SetText((int, float, float, float, float) val)
        {
            countText.text = val.Item1.ToString();
            weightText.text = val.Item3.ToString("0.00") + " кг";
            discountText.text = val.Item4.ToString("0.00") + " ₽";
            deliveryText.text = val.Item2 == 0 ? "Бесплатно" : (val.Item2.ToString("0.00") + " ₽");
            totalPriceText.text = (val.Item5+val.Item2).ToString("0.00") + " ₽";
        }
        
        private void Cancel(){}
        public void CleanBasket()
        {
            productParent.SetActive(false);
            DestroyOldProductList(productParent.transform);
            basketInteractor.CleanBasket();
            UpdateBottomInfo();
        }
        private void DestroyOldProductList(Transform parent)
        {
            foreach (Transform child in parent) Destroy(child.gameObject);
        }

        public void CreateBasket() => basketInteractor.CreateBasket();

        public void Continue()
        {
            var val = basketInteractor.GetCharacteristics();
            if (val.Item5 + val.Item2 > 1000)
            {
                changeViewInteractor.ChangeView(21);
            }
            else
            {
                
                notificationInteractor.ShowNotification(title, body);
            }
        }
    }
}