using System;
using System.Collections.Generic;
using Interactor;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Zenject;
using Button = UnityEngine.UI.Button;
using Toggle = UnityEngine.UI.Toggle;

namespace Delivery.Views
{
    public class TopMenu : MonoBehaviour
    {
        [SerializeField] private int startingView = 0;
        [SerializeField] private List<ViewTopInfo> TopInfos;
        [SerializeField] private GameObject Scan, Search, Like, Reset, TopBar, ShopLocation;
        [SerializeField] private TextMeshProUGUI Text;
        [SerializeField] private InputField searchBar;
        [SerializeField] private Button Back;

        private Toggle likeToggle;
        private Text placeholder;
        private int forceViewBack;
        private int currentView, previousView;
        private bool isPromotionView;

        [Inject] private ChangeViewInteractor changeViewInteractor;
        [Inject] private ShopInteractor shopInteractor;
        [Inject] private AccountInteractor accountInteractor;
        [Inject] private ShopLocationInteractor shopLocationInteractor;
        [Inject] private LoadingInteractor loadingInteractor;

        void Update()
        {
//#if UNITY_ANDROID
            AndroidBack();
//#endif
        }

        private void AndroidBack()
        {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            if (!TopInfos[currentView].NeedTop) return;
            if (!loadingInteractor.CurrentEqual(LoadingTrigger.Closed)) return;
            Back.onClick.Invoke();
        }
        
        void Awake()
        {
            likeToggle = Like.GetComponent<Toggle>();
            previousView = startingView;
            placeholder = searchBar.placeholder.GetComponent<Text>();
            changeViewInteractor.OnChangeView.Subscribe(OpenView);
            OpenView(shopLocationInteractor.HasLocation() ? startingView : 7);
        }
        public void SearchButton()
        {
            if (searchBar.gameObject.activeInHierarchy)
            {
                if (searchBar.text.IsNullOrWhitespace() )
                {
                    placeholder.text = "Требуется ввести запрос...";
                    searchBar.text = "";
                }
                else if (searchBar.text.Length < 3)
                {
                    placeholder.text = "В запросе должно быть больше двух символов...";
                    searchBar.text = "";
                }
                else ActivateSearch();

                return;
            }
            
            placeholder.text = "Поисковый запрос...";
            searchBar.gameObject.SetActive(true);
        }

        private void OpenView(int position)
        {
            CheckIfActivatePromotion(position);
            previousView = currentView;
            currentView = position;
            var cur = TopInfos[position];
            CheckIfShowLocation();
            if (!cur.NeedTop)
            {
                TopBar.SetActive(false);
                return;
            }
            TopBar.SetActive(true);
            searchBar.gameObject.SetActive(false);
            Like.SetActive(cur.NeedLike);
            if (cur.NeedScan)
            {
                likeToggle.SetIsOnWithoutNotify(false);
                Scan.SetActive(true);
                Search.SetActive(true);
                Reset.SetActive(false);
            } 
            else if (cur.NeedClean)
            {
                Scan.SetActive(false);
                Search.SetActive(false);
                Reset.SetActive(true);
            }
            else
            {
                Scan.SetActive(false);
                Search.SetActive(false);
                Reset.SetActive(false);
            }
            
            if(currentView != 3) DeactivateSearch();
            
            Back.onClick.RemoveAllListeners();
            Back.onClick.AddListener(()=>
            {
                changeViewInteractor.ChangeView(GetViewForReturn(cur.PreviousView));
            });

            SetName(position, cur.CurrentViewName);
        }

        private int GetViewForReturn(int naturalPrevious)
        {
            return HasSpecifiedReturnView() ? forceViewBack : 
                currentView == 30 || currentView == 7 ? previousView : naturalPrevious;
        }
        
        private bool HasSpecifiedReturnView()
        {
            if (!shopInteractor.IsSearching() && !isPromotionView || currentView == 9  || currentView == 10) 
                return false;
            isPromotionView = false;
            return true;
        }

        private void SetName(int position, string currentViewName)
        {
            if (position == 3 || position == 8) currentViewName = position == 8 ? 
                shopInteractor.GetCurrentCategory()?.Name : shopInteractor.GetSubCategory()?.Name;
            
            Text.text = currentViewName;
        }

        private void CheckIfActivatePromotion(int newPosition)
        {
            if (currentView != 0 || newPosition != 3) return;
            isPromotionView = true;
            forceViewBack = 0;
        }
        
        private void CheckIfShowLocation()
        {
            if (currentView != 0 || !accountInteractor.HasAccountEntity())
                ShopLocation.SetActive(currentView == 7);
            else ShopLocation.SetActive(shopLocationInteractor.HasLocation());
        }

        private void ActivateSearch()
        {
            shopInteractor.SetIsSearching(true);
            forceViewBack = currentView != 3 ? currentView : 8;
            changeViewInteractor.ChangeView(3);
            var searchString = searchBar.text;
            Text.text = searchString;
            loadingInteractor.StartLoad();
            shopInteractor.SearchForGoods(searchString);
        }
        
        private void DeactivateSearch()
        {
            shopInteractor.SetIsSearching(false);
            searchBar.text = "";
        }

        public void FavouriteFilter(bool val)
        {
            loadingInteractor.StartLoad();
            if (shopInteractor.IsSearching())
            {
                if (val) shopInteractor.SearchForFavouriteGoods(searchBar.text);
                else shopInteractor.SearchForGoods(searchBar.text);
            }
            else
            {
                if (val) shopInteractor.GetFavouriteGoods();
                else shopInteractor.GetGoods();
            }
        }
        
        [Serializable]
        public struct ViewTopInfo
        {
            public string CurrentViewName;
            public bool NeedTop;

            [HorizontalGroup("1"), ShowIf(nameof(NeedTop))]
            public bool NeedScan, NeedClean;
            [ShowIf(nameof(NeedTop))]
            public bool  NeedLike;
            [ValueDropdown(nameof(DropdownList))] public int PreviousView;
            [ValueDropdown(nameof(DropdownList))] public int CurrentView;

            private ValueDropdownList<int> DropdownList => ViewsController.ViewsDropdownList;
        }
        
    }
}