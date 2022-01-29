using UnityEngine;
using Interactor;
using TMPro;
using Zenject;

namespace Delivery.Views
{
    public class ProfileView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI Name, StoreLocation;

        [Inject] private AccountInteractor accountInteractor;
        [Inject] private ChangeViewInteractor changeViewInteractor;
        [Inject] private ShopLocationInteractor shopLocationInteractor;
        [Inject] private NotificationInteractor notificationInteractor;

        void OnEnable()
        {
            if (shopLocationInteractor.HasLocation()) StoreLocation.text = shopLocationInteractor.ShopName();
            if (!accountInteractor.TryGetAccountEntity(out var userData)) return;
            Name.text = userData.Name;
        }

        public void Logout()
        {
            notificationInteractor.ShowNotificationWithChoice(Reply.Logout, Reply.LogoutBody, () =>
            {
                accountInteractor.DeleteAccount();
                changeViewInteractor.ChangeView(5);
            }, () => { });
        }

        public void ChooseStorelocation()
        {
            changeViewInteractor.ChangeView(7);
        }
    }
}