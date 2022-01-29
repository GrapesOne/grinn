using System;
using UniRx;


namespace Interactor
{
    public class LoadingInteractor
    {
        private ISubject<LoadingTrigger> onStartLoad = new Subject<LoadingTrigger>();
        public IObservable<LoadingTrigger> OnStartLoad => onStartLoad;
        
        private ISubject<LoadingTrigger> onEndLoad = new Subject<LoadingTrigger>();
        public IObservable<LoadingTrigger> OnEndLoad => onEndLoad;
        private ISubject<LoadingTrigger> onClosedLoad = new Subject<LoadingTrigger>();
        public IObservable<LoadingTrigger> OnClosedLoad => onClosedLoad;
        private ISubject<LoadingTrigger> whenLongLoad= new Subject<LoadingTrigger>();
        public IObservable<LoadingTrigger> WhenLongLoad => whenLongLoad;

        private bool inLoad;
        public bool IsInLoad => inLoad;
        private LoadingTrigger current;
        public bool CurrentEqual(LoadingTrigger val) => current == val;
        
        public void LongLoad()
        {
            inLoad = true;
            whenLongLoad.OnNext(LoadingTrigger.LongLoad);
        }
        public void ClosedLoad()
        {
            inLoad = false;
            current = LoadingTrigger.Closed;
            onClosedLoad.OnNext(LoadingTrigger.Closed);
        }
        public void StartLoad()
        {
            inLoad = true;
            //Debug.Log("StartLoad");
            current = LoadingTrigger.Start;
            onStartLoad.OnNext(LoadingTrigger.Start);
        }

        public void EndLoad()
        {
            inLoad = false;
            //Debug.Log("EndLoad");
            current = LoadingTrigger.End;
            onEndLoad.OnNext(LoadingTrigger.End);
        }
    }
    public enum LoadingTrigger
    {
        Start,
        End,
        In,
        Closed,
        LongLoad
    }
}
