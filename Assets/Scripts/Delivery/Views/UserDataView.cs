using Interactor;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using Zenject;

namespace Delivery.Views
{
    public class UserDataView : MonoBehaviour
    {
        [Inject] private AccountInteractor accountInteractor;
        [SerializeField] private TextMeshProUGUI Name, BirthDate, Phone, Card, Email;

        void OnEnable()
        {
            if (!accountInteractor.TryGetAccountEntity(out var userData)) return;
            Name.text = userData.Name;
            Phone.text = userData.Phone;
            Card.text = userData.Card.IsNullOrWhitespace() ? "" : "*********" + userData.Card.Substring(9);
            Email.text = userData.Email;
        }
        
    }
}