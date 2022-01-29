using System;
using System.Threading;
using Entity;
using UniRx;

namespace Interactor
{
    public class NotificationInteractor
    {
        private ISubject<NotificationEntity> onNotification = new Subject<NotificationEntity>();
        public IObservable<NotificationEntity> OnNotification => onNotification;

        private ISubject<(NotificationEntity, NotificationHandler)> onInterctiveNotification
            = new Subject<(NotificationEntity, NotificationHandler)>();

        public IObservable<(NotificationEntity, NotificationHandler)> OnInterctiveNotification
            => onInterctiveNotification;

        private ISubject<(NotificationEntity, NotificationAcceptor)> onAcceptableNotification
            = new Subject<(NotificationEntity, NotificationAcceptor)>();

        public IObservable<(NotificationEntity, NotificationAcceptor)> OnAcceptableNotification
            => onAcceptableNotification;

        public void NoInternetNotification() =>
            onNotification.OnNext(new NotificationEntity
                {Title = Reply.NoInternetConnection, Body = Reply.CheckConnection});

        public void ShowNotification(string title, string body) =>
            onNotification.OnNext(new NotificationEntity {Title = title, Body = body});

        public void ShowNotificationWithChoice(string title, string body, Action OnSuccess, Action OnFailure)
        {
            onInterctiveNotification.OnNext((new NotificationEntity {Title = title, Body = body},
                new NotificationHandler(OnSuccess, OnFailure)));
        }

        public void ShowNotificationWithAccept(string title, string body, Action OnAccept)
        {
            onAcceptableNotification.OnNext((new NotificationEntity {Title = title, Body = body},
                new NotificationAcceptor(OnAccept)));
        }


        public class NotificationAcceptor
        {
            private Action Accept;
            public NotificationAcceptor(Action OnAccept) => Accept = OnAccept;
            public void CallAccept() => Accept?.Invoke();
        }

        public class NotificationHandler
        {
            private Action Success, Failure;

            public NotificationHandler(Action OnSuccess, Action OnFailure)
            {
                Success = OnSuccess;
                Failure = OnFailure;
            }

            public void CallSuccess() => Success?.Invoke();
            public void CallFailure() => Failure?.Invoke();
        }
    }

    public static class Reply
    {
        public const string CannotCreateOrder = "Невозможно создать заказ";
        public const string NoProducts = "В корзине должны присутствовать товары для заказа";
        public const string NotAuthorized = "Авторизуйтесь для продолжения заказа";
        public const string NoAddress = "Нет адреса для доставки";

        public const string NoInternetConnection = "Не могу подключиться к интернету";
        public const string CheckConnection = "Проверьте ваше интернет-соединение";

        public const string Logout = "Выход";
        public const string LogoutBody = "Вы покинете текущий аккаунт";

        public const string Attention = "Внимание!";
        public const string SectionIsNotReady = "Данный раздел временно работает не полностью по техническим причинам";
        public const string NoDataFromServer = "Этих данных нет на сервере";
        public const string NoDesign = "Нет дизайна для данного экрана";
        public const string IsDemo = "Сценарий временно недоступен";
    }
}