using System.Collections;
using System.Collections.Generic;
using Interactor;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ShowNotificationButtonHandler : MonoBehaviour
{
    [Inject] private NotificationInteractor interactor;
    [SerializeField] private bool isToggle, isButton;
    [SerializeField] private string title, body;
    [SerializeField] private bool acceptable;

    void Awake()
    {
        if(isButton) GetComponent<Button>().onClick.AddListener(Call);
        if(isToggle) GetComponent<Toggle>().onValueChanged.AddListener(Call);
        if (!isButton && !isToggle) Call();
    }
    void Call()
    {
        if (acceptable) interactor.ShowNotificationWithAccept(title, body, () => { });
        else interactor.ShowNotification(title, body);
    }
    void Call(bool val)
    {
        if(!val) return;
        if (acceptable) interactor.ShowNotificationWithAccept(title, body, () => { });
        else interactor.ShowNotification(title, body);
    }
   
}
