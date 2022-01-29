using Entity;
using Interactor;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Delivery.Views
{
    public class Register : MonoBehaviour
    {
        [SerializeField] private InputFieldValidator Email;
        [SerializeField] private InputFieldValidator Pw;
        [SerializeField] private InputFieldValidator PwConfirm;
        [SerializeField] private InputFieldValidator Name;
        [SerializeField] private InputFieldValidator Phone;
        [SerializeField] private InputFieldValidator Card;
        [SerializeField] private Toggle Terms;
        [SerializeField] private Button Deactivate, Activate;
        
        [Inject] private AccountInteractor accountInteractor;


        public void WhatIsWrong()
        {
            Colorize(Email);
            Colorize(Pw);
            Colorize(PwConfirm);
            Colorize(Name);
            Colorize(Phone);
            if(Pw.Text != PwConfirm.Text) PwConfirm.SetColor(new Color(0.9811321f,0.5609114f,0.5609114f));
            if(!Terms.isOn) Terms.GetComponent<Animator>().SetTrigger("Pressed");
        }

        private static void Colorize(InputFieldValidator validator)
        {
            if (!validator.IsValid) validator.SetColor(new Color(0.9811321f,0.5609114f,0.5609114f));
        }
        
        public void UpdateRegisterButton()
        {
            if (Email.IsValid && Pw.IsValid && PwConfirm.IsValid && Phone.IsValid && 
                PwConfirm.Text == Pw.Text && Name.IsValid && Terms.isOn)
            {
                Deactivate.gameObject.SetActive(false);
                Activate.gameObject.SetActive(true);
            }
            else
            {
                Deactivate.gameObject.SetActive(true);
                Activate.gameObject.SetActive(false);
            }
        }
        public void RegisterButton()
        {
            accountInteractor.SendRegister(GatherRegistrationDetails());
        }

        private AccountEntity GatherRegistrationDetails()
        {
            Debug.Log(Email.Text+"\n"+
                      Pw.Text+"\n"+
                      PwConfirm.Text+"\n"+
                      Name.Text+"\n"+
                      Phone.Text+"\n"+
                      Card.Text+"\n");
            return new AccountEntity
            {
                Email = Email.Text,
                Pw = Pw.Text,
                PwConfirm = PwConfirm.Text,
                Name = Name.Text,
                Phone = Phone.Text,
                Card = Card.Text.Replace(" ", ""),
                Terms = Terms.isOn
            };
        }
    }
}