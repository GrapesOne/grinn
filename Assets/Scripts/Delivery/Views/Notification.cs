using System;
using Entity;
using Interactor;
using TMPro;
using UnityEngine;
using Zenject;
using UniRx;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Delivery.Views
{
    public class Notification : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI bodyText;
        [SerializeField] private GameObject TwoButtonsBlock, OneButtonBlock;
        [SerializeField] private Button Activate, Deactivate, Accept;
        private Animator _animator;
        
        [Inject] private NotificationInteractor notificationInteractor;

        void Start()
        {
            _animator = GetComponent<Animator>();
            notificationInteractor.OnNotification.Subscribe(DisplayNotification);
            notificationInteractor.OnInterctiveNotification.Subscribe(DisplayInteractiveNotification);
            notificationInteractor.OnAcceptableNotification.Subscribe(DisplayAcceptableNotification);
        }
        private void DisplayAcceptableNotification((NotificationEntity notifData,
            NotificationInteractor.NotificationAcceptor handler) val)
        {
            RoutineSet(val.notifData, false, true, "Start");
            SetToButton(Accept, val.handler.CallAccept);
        }

        private void DisplayInteractiveNotification((NotificationEntity notifData,
            NotificationInteractor.NotificationHandler handler) val)
        {
            RoutineSet(val.notifData, true, false, "Start");
            SetToButton(Activate, val.handler.CallSuccess);
            SetToButton(Deactivate, val.handler.CallFailure);
        }
        
        private void DisplayNotification(NotificationEntity notifData) 
            => RoutineSet(notifData, false, false, "Fast");
        
        private void RoutineSet(NotificationEntity notifData, bool a, bool b, string trigger)
        {
            titleText.text = notifData.Title;
            bodyText.text = notifData.Body;
            TwoButtonsBlock.SetActive(a);
            OneButtonBlock.SetActive(b);
            _animator.SetTrigger(trigger);
        }
        private void SetToButton(Button button, UnityAction action)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => _animator.SetTrigger("Close"));
            button.onClick.AddListener(action);
        }
    }
}