using System;
using System.Collections;
using Interactor;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Zenject;

namespace Delivery.Views
{
    public class ViewsController : MonoBehaviour
    {
        [SerializeField] private GameObject[] views;

        [SerializeField] private ViewObject[] Views;

        [Inject] private ChangeViewInteractor changeViewInteractor;
        [Inject] private AccountInteractor accountInteractor;
        [Inject] private ShopLocationInteractor shopLocationInteractor;
        [Inject] private NotificationInteractor notificationInteractor;

        private int startingView = 0;

        [Button] void MakeStatic() => ViewsDropdownList = viewsDropdownList;

        void Start()
        {
            MakeStatic();
            changeViewInteractor.OnChangeView.Subscribe(OpenView);
            OpenView(shopLocationInteractor.HasLocation() ? startingView : 7);
        }

#if UNITY_EDITOR
        //TODO remove this, for testing shop location only.
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                shopLocationInteractor.DeleteShopLocation();
            }
        }

        [Button]
        void ResetShop()
        {
            shopLocationInteractor.DeleteShopLocation();
        }
#endif
        
       

        private void DeactivateAllViews()
        {
            foreach (var view in Views)
            {
                view.View.SetActive(false);
            }
        }    

        public void OpenView(int position)
        {
            DeactivateAllViews();
            if(Views[position].RequestAuthorization)
                if (!accountInteractor.HasAccountEntity())
                {
                    notificationInteractor.ShowNotification("Внимание", "Для просмотра данного окна необходимо авторизоваться");
                    StartCoroutine(OpenViewLogin());
                    return;
                }
            Views[position].View.SetActive(true);
        }

        private IEnumerator OpenViewLogin()
        {
            yield return new WaitForFixedUpdate();
            changeViewInteractor.ChangeView(5);
        } 
        
        public static ValueDropdownList<int> ViewsDropdownList; 
        private ValueDropdownList<int> viewsDropdownList 
        {
            get
            {
                var dropdownList = new ValueDropdownList<int>();
                for (var i = 0; i < Views.Length; i++)
                {
                    const int maxSubstringLength = 30;
                    var substringLength = Mathf.Min(Views[i].View.name.Length, maxSubstringLength);
                    var dropdownLabel = Views[i].View.name.Substring(0, substringLength);
                    dropdownList.Add($"{i}: {dropdownLabel}", i);
                }

                return dropdownList;
            }
        }
    }

    [Serializable]
    public struct ViewObject
    {
        public bool RequestAuthorization;
        public GameObject View;
    }
    
}
