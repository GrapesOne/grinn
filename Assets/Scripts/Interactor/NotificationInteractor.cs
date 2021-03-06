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
        public const string CannotCreateOrder = "???????????????????? ?????????????? ??????????";
        public const string NoProducts = "?? ?????????????? ???????????? ???????????????????????????? ???????????? ?????? ????????????";
        public const string NotAuthorized = "?????????????????????????? ?????? ?????????????????????? ????????????";
        public const string NoAddress = "?????? ???????????? ?????? ????????????????";

        public const string NoInternetConnection = "???? ???????? ???????????????????????? ?? ??????????????????";
        public const string CheckConnection = "?????????????????? ???????? ????????????????-????????????????????";

        public const string Logout = "??????????";
        public const string LogoutBody = "???? ???????????????? ?????????????? ??????????????";

        public const string Attention = "????????????????!";
        public const string SectionIsNotReady = "???????????? ???????????? ???????????????? ???????????????? ???? ?????????????????? ???? ?????????????????????? ????????????????";
        public const string NoDataFromServer = "???????? ???????????? ?????? ???? ??????????????";
        public const string NoDesign = "?????? ?????????????? ?????? ?????????????? ????????????";
        public const string IsDemo = "???????????????? ???????????????? ????????????????????";
    }
}