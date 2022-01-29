using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Delivery.Prefabs;
using Entity;
using Interactor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;

namespace Delivery.Views
{
    public class MainPage : MonoBehaviour
    {
        [SerializeField] private GameObject[] GOForNoname, GOForUser;
        [SerializeField] private TextMeshProUGUI Name;
        [SerializeField] private TextMeshProUGUI ShopLocationText;

        [SerializeField] private Transform ContentPromotion, ContentPopular, ContentProfitable;
        [SerializeField] private GameObject ProductPrefab;
        
        private CatalogEntity catalog;
        private List<ProductEntity> _products = new List<ProductEntity>();
        private List<ProductEntity> bestOffer = new List<ProductEntity>();
        private List<ProductEntity> frequent = new List<ProductEntity>();
        private List<ProductPrefab> productsList = new List<ProductPrefab>();

        [Inject] private AccountInteractor accountInteractor;
        [Inject] private ShopLocationInteractor shopLocationInteractor;
        [Inject] private ChangeViewInteractor changeViewInteractor;
        [Inject] private ShopInteractor shopInteractor;
        [Inject] private BasketInteractor basketInteractor;
        [Inject] private FavouritesInteractor favouritesInteractor;
        [Inject] private DownloadInteractor downloadInteractor;
        [Inject] private LoadingInteractor loadingInteractor;

        private int i , j, anchor, state;
        private CancellationTokenSource  _cancellationTokenSource = new CancellationTokenSource();
        
        void OnEnable()
        {
            if (!loadingInteractor.IsInLoad) loadingInteractor?.StartLoad();
            var hasAccount = accountInteractor.TryGetAccountEntity(out var userData);
            if (hasAccount)
            {
                var firstSpace = userData.Name.IndexOf(' ') > 0 ? userData.Name.IndexOf(' ') : userData.Name.Length;
                var username = userData.Name.Substring(0, firstSpace);
                Name.text = $"Здравствуйте, {username}!";
                if (shopLocationInteractor.HasLocation())
                {
                    ShopLocationText.text = shopLocationInteractor.ShopName();
                    basketInteractor.SetAddress(shopLocationInteractor.ShopName());
                }
            }
          
            foreach (var o in GOForUser) o.SetActive(hasAccount);
            foreach (var o in GOForNoname) o.SetActive(!hasAccount);
            
            foreach (var t in productsList) t.UpdateProduct();
            switch (state)
            {
                case 0:
                    _cancellationTokenSource = new CancellationTokenSource();
                    StartCoroutine(AwaitCatalog(_cancellationTokenSource.Token));
                    break;
                case 1:
                    InitNumberOfSpecialCategories();
                    RequestGoods();
                    break;
                case 2:
                    DestroyOldContent(ContentPopular);
                    CreateFrom(frequent, ContentPopular);
                    DestroyOldContent(ContentProfitable);
                    CreateFrom(bestOffer, ContentProfitable);
                    break;
                case 3:
                    loadingInteractor.EndLoad();
                    break;
            }
        }

        void OnDisable() => _cancellationTokenSource.Cancel();

        void Start()
        {
            shopInteractor.OnCatalogUpdate.Subscribe(GetCategories);
            shopInteractor.OnGotProducts.Subscribe(GetProducts);
        }

        private IEnumerator AwaitCatalog(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (catalog == null) shopInteractor.GetLatestCatalog();
                else break;
                yield return new WaitForSeconds(8);
            }
        }

        void InitNumberOfSpecialCategories()
        {
            i = Random.Range(0, 4) switch
            {
                0 => Random.Range(0, 2) == 0 ? 0 : 4,
                1 => Random.Range(6, 10),
                2 => Random.Range(12, 19),
                3 => Random.Range(22, 24),
                _ => 13
            };
            anchor = i + 1;
            j = 0;
        }
        private void GetCategories(CatalogEntity newCatalog)
        {
            catalog = newCatalog;
            if (state == 0) state = 1;
            if (catalog?.Categories == null) return;
            InitNumberOfSpecialCategories();
            RequestGoods();
        }

        private bool RequestGoods()
        {
            if (!gameObject.activeSelf) return true;
            if (i == anchor) return true;
            if (j >= catalog.Categories[i].Children.Length)
            {
                j = 0;
                i++;
                shopInteractor.SetCurrentCategory(catalog.Categories[i]);
                return RequestGoods();
            }
            if (i >= catalog.Categories.Length) return false;
            shopInteractor.SetCurrentSubCategory(catalog.Categories[i].Children[j]);
            shopInteractor.GetGoods();
            j++;
            return false;
        }

        private void GetProducts(ProductEntity[] products)
        {
            _products.AddRange(products);
            if (!RequestGoods()) return;
            Find();
            if (state == 1) state = 2;
            if(_cancellationTokenSource.IsCancellationRequested) return;
            DestroyOldContent(ContentPopular);
            CreateFrom(frequent, ContentPopular);
            DestroyOldContent(ContentProfitable);
            CreateFrom(bestOffer, ContentProfitable);
            if (state == 2) state = 3;
            loadingInteractor.EndLoad();
        }
        private void Find()
        {
            int k = Random.Range(0,5), l = 6, count = 10;
            foreach (var product in _products)
            {
                if (--k <= 0)
                {
                    k = Random.Range(0, 6);
                    continue;
                }
                if (count != bestOffer.Count &&
                    float.Parse(product.ActPrice, CultureInfo.InvariantCulture) 
                    < float.Parse(product.RegPrice, CultureInfo.InvariantCulture))
                    bestOffer.Add(product);

                if (count == frequent.Count || --l > 0) continue;
                l = 6;
                frequent.Add(product);
            }
        }


        private void CreateFrom(List<ProductEntity> entities, Transform productParent)
        {
            foreach (var item in entities)
            {
                var temp = Instantiate(ProductPrefab, productParent.transform, false);
                var pp = temp.GetComponent<ProductPrefab>();
                pp.SetupFavouritesInteractor(favouritesInteractor);
                pp.SetupBasketInteractor(basketInteractor);
                pp.SetupProduct(item);
                downloadInteractor.GetImage("https://linia-market.ru" + item.Image, imageTexture => pp.SetImage(imageTexture));
                productsList.Add(pp);
            }
        }
        private void DestroyOldContent(Transform parent)
        {
            foreach (Transform child in parent) Destroy(child.gameObject);
        }
        public void ToPromotions() //100 products 30% off
        {
            GoToSubSection(0);
        }

        public void ToWeeklyProducts()
        {
            GoToSubSection(1);
        }

        public void ToProfitableOffers()
        {
            GoToSubSection(2);
        }

        private void GoToSubSection(int category)
        {
            if (catalog == null)
            {
                Debug.LogWarning("Trying to reach promo subsection, but category has not loaded yet.");
                return;
            }
            
            shopInteractor.SetCurrentCategory(catalog.Categories[category]);
            shopInteractor.SetCurrentSubCategory(catalog.Categories[category].Children[0]);
            changeViewInteractor.ChangeView(3);
        }
    }
}