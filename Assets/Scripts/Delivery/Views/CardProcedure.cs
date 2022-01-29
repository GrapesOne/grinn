using Entity;
using Interactor;
using Sirenix.Utilities;
using UnityEngine;
using Zenject;

namespace Delivery.Views
{
    public class CardProcedure : MonoBehaviour
    {
        [Inject] private AccountInteractor accountInteractor;
        [Inject] private ChangeViewInteractor changeViewInteractor;
        void OnEnable()
        {
            if(accountInteractor.HasCard()) changeViewInteractor.ChangeView(16);
        }
    }
}