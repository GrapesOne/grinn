using System.Collections.Generic;
using System.Linq;
using Entity;
using Interactor;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using Zenject;

namespace Delivery.Views
{
    public class SelectStore : MonoBehaviour
    {
        [SerializeField] private TextAsset Asset;
        [SerializeField] private ToggleGroup toggleGroup;
        [SerializeField] private GameObject togglePrefab;
        [SerializeField] private Dropdown Cities;

        [Inject] private ChangeViewInteractor changeViewInteractor;
        [Inject] private ShopLocationInteractor shopLocationInteractor;
        [Inject] private BasketInteractor basketInteractor;
        [Inject] private FavouritesInteractor favouritesInteractor;
        [Inject] private NotificationInteractor notificationInteractor;

        private ShopEntity shopEntity;
        private TopologyEntity topology;
        private int storeId;
        void Awake()
        {
            PlayerPrefs.SetString("Topology", Asset.text);
            PlayerPrefs.Save();
            JsonUtility.Deserialize<TopologyEntity>(Asset.text, FillDropdown);
        }

        private void FillDropdown(TopologyEntity topologyEntity)
        {
            topology = topologyEntity;
            shopLocationInteractor.SetTopology(topology);
            Cities.ClearOptions();
            var list = topology.cities.ToList();
            list.Add("");
            Cities.AddOptions(list);
            Cities.value = 0;
            Cities.onValueChanged.RemoveAllListeners();
            Cities.onValueChanged.AddListener(UpdateValue);
            UpdateValue(0);
        }

        public void UpdateValue(int val)
        {
            foreach (Transform toggle in toggleGroup.transform)
            {
                toggleGroup.UnregisterToggle(toggle.GetComponent<Toggle>());
                Destroy(toggle.gameObject);
            }
            var city = Cities.options[val].text;
            var shops = topology.shops.Where(shop => shop.city == city).ToList();
           
            foreach (var shop in shops)
            {
                var go = Instantiate(togglePrefab, toggleGroup.transform);
                var toggle = go.GetComponent<Toggle>();
                toggle.group = toggleGroup;
                toggleGroup.RegisterToggle(toggle);
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener(val => { if (val) SetStore(shop.id); });
                var text = go.GetComponentInChildren<TextMeshProUGUI>();
                text.text = shop.name + ", " + shop.address;
                var fitter = text.GetComponent<TmpContentSizeFitter>();
                fitter.SetValues(charSizeConstY: 40, minSizeParentY: 0);
            }

            toggleGroup.GetFirstActiveToggle().onValueChanged.Invoke(true);
        }
        public void SetStore(int id)
        {
            storeId = id;
        }
        private void Accept()
        {
            favouritesInteractor.CleanBasket();
            basketInteractor.CleanBasket();
            Debug.Log("Chose store: " + storeId + ", " + shopEntity.address);
            shopLocationInteractor.SetShopLocation(shopEntity);
            changeViewInteractor.ChangeViewToPrevious();
        }
        private void Cancel()
        {
            Debug.Log("Canceled");
        }
        public void SelectChosenStore()
        {
            shopEntity = topology.shops.First( shop => shop.id == storeId);
            if (shopLocationInteractor.ShopId() != shopEntity.id)
            {
                if (basketInteractor.GetBasketCount() != 0 || favouritesInteractor.GetBasketCount() != 0)
                {
                    notificationInteractor.ShowNotificationWithChoice(
                        Reply.Attention, "Вы удалите все продукты из корзины и избранного, " +
                                         "так как товары в разных филиалах отличаются",
                        Accept, Cancel);
                    return;
                }
            }
            Debug.Log("Chose store: " + storeId + ", " + shopEntity.address);
            shopLocationInteractor.SetShopLocation(shopEntity);
            changeViewInteractor.ChangeViewToPrevious();
            
        }
    }
}