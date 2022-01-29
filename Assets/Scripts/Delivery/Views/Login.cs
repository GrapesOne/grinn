using Entity;
using Interactor;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Delivery.Views
{
    public class Login : MonoBehaviour
    {
        [SerializeField] private TMP_InputField Email;
        [SerializeField] private InputField Pw;

        [Inject] private ChangeViewInteractor changeViewInteractor;
        [Inject] private AccountInteractor accountInteractor;

#if UNITY_EDITOR
        [Button]
        public void AutoLoginButtonDebug(string email = "netumbartseva@gmail.com", string password = "PpoE405060")
        {
            Email.text = email;
            Pw.text = password;
            accountInteractor.SendLogin(GatherLoginDetails());
        }
#endif


        public void RegisterButton()
        {
            changeViewInteractor.ChangeView(6);
        }

        public void LoginButton()
        {
            accountInteractor.SendLogin(GatherLoginDetails());
        }

        private AccountEntity GatherLoginDetails()
        {
            return new AccountEntity()
            {
                Email = Email.text,
                Pw = Pw.text
            };
        }
    }
}