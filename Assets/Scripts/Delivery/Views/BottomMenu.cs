using System;
using System.Collections.Generic;
using Entity;
using Interactor;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Delivery.Views
{
    public class BottomMenu : MonoBehaviour
    {
        [SerializeField] private List<BottomToggleButton> BottomToggleButtons;
        [SerializeField] private GameObject BasketCounter;
        [SerializeField] private TextMeshProUGUI BasketCounterText;

        [Inject] private AccountInteractor accountInteractor;
        [Inject] private ChangeViewInteractor changeViewInteractor;
        [Inject] private ShopLocationInteractor shopLocationInteractor;
        [Inject] private NotificationInteractor notificationInteractor;
        [Inject] private BasketInteractor basketInteractor;

        private Color off, on;

        void Awake()
        {
            basketInteractor.OnAddToBasket.Subscribe(UpdateBasketCounter);
            basketInteractor.OnRemoveFromBasket.Subscribe(UpdateBasketCounter);
            basketInteractor.OnCleanBasket.Subscribe(UpdateBasketCounter);
            accountInteractor.OnAccountUpdate.Subscribe(UpdateBasketCounter);
            var c = BottomToggleButtons[0].backgroundImage.color;
            on = c;
            c.a = 0;
            off = c;
            ActivateIcon(0);
            changeViewInteractor.OnChangeView.Subscribe(ActivateIcon);
            foreach (var button in BottomToggleButtons)
            {
                var b = button.backgroundImage.GetComponent<Button>();
                b.onClick.RemoveAllListeners();
                b.onClick.AddListener(() => GoToView(button.View));
            }
        }

        void Start()
        {
            UpdateBasketCounter();
        }

        private void UpdateBasketCounter(AccountEntity _) => UpdateBasketCounter();
        private void UpdateBasketCounter(ProductEntity _) => UpdateBasketCounter();
        private void UpdateBasketCounter(int _) => UpdateBasketCounter();

        private void UpdateBasketCounter()
        {
            var count = basketInteractor.GetBasketCount();
            BasketCounter.SetActive(count != 0);
            BasketCounterText.text = count.ToString();
        }

        private void ActivateIcon(int position)
        {
            foreach (var button in BottomToggleButtons)
            {
                if (button.View == position)
                {
                    button.backgroundImage.color = on;
                    button.iconImage.sprite = button.activeSprite;
                }
                else
                {
                    button.backgroundImage.color = off;
                    button.iconImage.sprite = button.deactiveSprite;
                }
            }
        }

        public void GoToView(int position)
        {
            if (!shopLocationInteractor.HasLocation())
            {
                notificationInteractor.ShowNotification("Внимание",
                    "Для продолжения необходимо выбрать магазин");
                return;
            }

            changeViewInteractor.ChangeView(position);
        }

        [Serializable]
        public struct BottomToggleButton
        {
            [ValueDropdown(nameof(DropdownList))] public int View;
            private ValueDropdownList<int> DropdownList => ViewsController.ViewsDropdownList;
            public Image iconImage, backgroundImage;
            public Sprite activeSprite, deactiveSprite;
        }
    }
}