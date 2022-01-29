using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Delivery.Prefabs;
using Entity;
using Interactor;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using Zenject;
using UniRx;
using UnityEngine.UI;

namespace Delivery.Views
{
    public class CatalogProducts : MonoBehaviour
    {
        [SerializeField] private Loading loading;
        [SerializeField] private GameObject productPrefab;
        [SerializeField] private GameObject productParent;
        [SerializeField] private Image SortingIcon;
        [SerializeField] private TextMeshProUGUI SortingText;
        
        [Inject] private ChangeViewInteractor changeViewInteractor;
        [Inject] private ShopInteractor shopInteractor;
        [Inject] private DownloadInteractor downloadInteractor;
        [Inject] private BasketInteractor basketInteractor;
        [Inject] private FavouritesInteractor favouritesInteractor;
        [Inject] private NotificationInteractor notificationInteractor;
        [Inject] private LoadingInteractor loadingInteractor;

        private bool hasProducts;
        public void OnEnable()
        {
            StartCoroutine(OnEnableCoroutine());
        }

        IEnumerator OnEnableCoroutine()
        {
            if(!loadingInteractor.IsInLoad) loadingInteractor.StartLoad();
            yield return StartCoroutine(Pool.CallPutChildesCoroutine(productParent.transform, 50));
            SetupProducts();
            StartCoroutine(AwaitCatalog());
        }
        public void UpdateSorting(Sprite icon, string text)
        {
            SortingIcon.sprite = icon;
            SortingText.text = text;
        }
        private IEnumerator AwaitCatalog()
        {
            while (true)
            {
                yield return new WaitForSeconds(8);
                if (!hasProducts) shopInteractor.GetGoods();
                else break;
            }
        }
        public void OnDisable()
        {
            Pool.CallPutChildes(productParent.transform,1);
            hasProducts = false;
        }
        
        void Start()
        {
            shopInteractor.OnSearchSuccess.Subscribe(UpdateProducts);
            shopInteractor.OnGotProducts.Subscribe(UpdateProducts);
        }
        
        private void SetupProducts()
        {
            shopInteractor.GetGoods();
        }
        
        private void UpdateProducts(ProductEntity[] products)
        {
            Pool.CallPutChildes(productParent.transform, 50);
            hasProducts = true;
            if (products == null || products.Length == 0)
            {
                notificationInteractor.ShowNotification("Ой!", "В данном разделе пока пусто");
                Debug.LogError("Error. Product catalog is empty!");
                loadingInteractor.EndLoad();
                return;
            }
            StopCoroutine(AwaitCatalog());
            StartCoroutine(Creation(products));
        }

        IEnumerator Creation(ProductEntity[] products)
        {
            if(!loadingInteractor.IsInLoad) loadingInteractor.LongLoad();
            while (productParent.transform.childCount > 0) yield return new WaitForEndOfFrame();
            //var size = 1;
            //var count = Mathf.CeilToInt(products.Length / (float)size);
            var text =  products.Length > 100 ? "Много товаров\nподождите, пожалуйста...\n \n" : Loading.BaseText + "\n \n";
            var layoutGroup = productParent.GetComponent<VerticalLayoutGroup>();
            layoutGroup.enabled = false;
            for (var i = 0; i < products.Length; i++)
            {
                loading?.SetText(text + (int)((float)i/products.Length*100) + "%");
                var index =  i ;
                var temp = Pool.GetGameObject(productPrefab, productParent.transform);
                var pp = temp.GetComponent<ProductPrefab>();
                pp.SetupFavouritesInteractor(favouritesInteractor);
                pp.SetupBasketInteractor(basketInteractor);
                pp.SetupProduct(products[index]);
                downloadInteractor.GetImage("https://linia-market.ru" + products[index].Image, 
                    imageTexture => pp.SetImage(imageTexture));
                yield return new WaitForEndOfFrame();
            }
            
            /*
            for (var i = 0; i < count; i++)
            {
                loading?.SetText(text + (int)((float)i/count*100) + "%");
                var step = products.Length - i * size ;
                for (var j = 0; j < step && j < size; j++)
                {
                    var index = j + i * size;
                    var temp = Pool.GetGameObject(productPrefab, productParent.transform);
                    var pp = temp.GetComponent<ProductPrefab>();
                    pp.SetupFavouritesInteractor(favouritesInteractor);
                    pp.SetupBasketInteractor(basketInteractor);
                    pp.SetupProduct(products[index]);
                    downloadInteractor.GetImage("https://linia-market.ru" + products[index].Image, 
                        imageTexture => pp.SetImage(imageTexture));
                }
                yield return new WaitForEndOfFrame();
            }*/
            layoutGroup.enabled = true;
            foreach (Transform t in productParent.transform)
            {
                t.gameObject.SetActive(true);
            }
            loadingInteractor.EndLoad();
        }
       
        public void GoBackToSubCatalog() => changeViewInteractor.ChangeView(8);

    }
}