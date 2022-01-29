using Interactor;
using UnityEngine;
using Zenject;

namespace Delivery.Views
{
    public class Filter : MonoBehaviour
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
        public void CallAttentionNoDesign()
        {
            notificationInteractor.ShowNotification(Reply.Attention,Reply.NoDesign);
        }
    }
}