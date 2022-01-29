using Interactor;
using UnityEngine;
using Zenject;

namespace Delivery.Views
{
    public class BrandName : MonoBehaviour
    {
        [Inject] private NotificationInteractor notificationInteractor;

        void OnEnable()
        {
            notificationInteractor.ShowNotification(Reply.Attention,Reply.SectionIsNotReady);
        }

        public void CallAttentionNoData()
        {
            notificationInteractor.ShowNotification(Reply.Attention,Reply.NoDataFromServer);
        }
        
    }
}