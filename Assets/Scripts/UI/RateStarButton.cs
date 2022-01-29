using System.Collections;
using System.Collections.Generic;
using Interactor;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class RateStarButton : MonoBehaviour
{
    [SerializeField] private int i;
    [SerializeField] private RateStarButton previous, next;
    [SerializeField] private Button rate;
    [SerializeField] private Color On, Off;
    [Inject] private NotificationInteractor _notificationInteractor;
    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
        GetComponent<Button>().onClick.AddListener(SetStar);
    }

    private void SetStar()
    {
        SetStar(true, true);
    }

    public void SetStar(bool v, bool start)
    {
        if (v)
        {
            image.color = On;
            previous?.SetStar(true, false);
            if (!start) return;
            rate.interactable = true;
            rate.onClick.RemoveListener(Rate);
            rate.onClick.AddListener(Rate);
            next?.SetStar(false, false);
        }
        else
        {
            image.color = Off;
            next?.SetStar(false, false);
        }
    }

    public void Rate()
    {
        _notificationInteractor.ShowNotification(Reply.Attention, Reply.IsDemo);
        //TODO write code for rate app
    }
}