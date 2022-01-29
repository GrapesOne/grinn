using System;
using Entity;
using Gateway;
using Sirenix.Utilities;
using UniRx;
using UnityEngine;
using Zenject;

namespace Interactor
{
    public class AccountInteractor
    {
        private ISubject<AccountEntity> onAccountUpdate = new Subject<AccountEntity>();
        public IObservable<AccountEntity> OnAccountUpdate => onAccountUpdate;
        [Inject] private AccountGateway accountGateway;
        [Inject] private ChangeViewInteractor changeViewInteractor;
        [Inject] private NotificationInteractor notificationInteractor;
        [Inject] private LoadingInteractor loadingInteractor;

        private AccountEntity _accountEntity;

        public void DeleteAccount()
        {
            _accountEntity = null;
            onAccountUpdate.OnNext(null);
        }

        public bool TryGetAccountEntity(out AccountEntity output)
        {
            if (_accountEntity == null)
            {
                output = new AccountEntity();
                return false;
            }

            output = _accountEntity;
            return true;
        }

        public bool HasAccountEntity() => _accountEntity != null;
        public bool HasCard() => _accountEntity != null && !_accountEntity.Card.IsNullOrWhitespace();

        public void SendLogin(AccountEntity details)
        {
            loadingInteractor.StartLoad();
            accountGateway.Login(details, OnLoginSuccess, OnLoginError);
        }

        public void SendRegister(AccountEntity details)
        {
            loadingInteractor.StartLoad();
            accountGateway.Register(details, OnRegisterSuccess, OnRegisterError);
        }

        private void OnLoginSuccess(AccountEntity details)
        {
            _accountEntity = details;
            onAccountUpdate.OnNext(_accountEntity);
            Debug.LogWarning("Login success! Email: " + details.Email + ", Name: " + details.Name + ", Phone: " +
                             details.Phone + ", Card: " + details.Card);
            notificationInteractor.ShowNotification("Успешный вход в систему",
                "Добро пожаловать на платформу онлайн-покупок Linya!");
            changeViewInteractor.ChangeView(0);
            loadingInteractor.EndLoad();
        }

        private void OnLoginError(string error)
        {
            Debug.LogError("Login error. " + error);
            notificationInteractor.ShowNotificationWithAccept("Ошибка входа", error, () => { });
            loadingInteractor.EndLoad();
        }

        private void OnRegisterSuccess(string result)
        {
            Debug.LogWarning("Register success! " + result);
            notificationInteractor.ShowNotificationWithAccept(
                "Успешная Регистрация", "Пожалуйста, проверьте свою электронную почту, чтобы подтвердить регистрацию.",
                () => { });
            changeViewInteractor.ChangeView(5);
            loadingInteractor.EndLoad();
        }

        private void OnRegisterError(string error)
        {
            Debug.LogError("Register error. " + error);
            notificationInteractor.ShowNotification("Ошибка Pегистрации", error);
            loadingInteractor.EndLoad();
        }
    }
}