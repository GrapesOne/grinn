using System;
using UniRx;

namespace Interactor
{
    public class ChangeViewInteractor
    {
        private ISubject<int> onChangeView = new Subject<int>();
        public IObservable<int> OnChangeView => onChangeView;

        private int previous = 0, current = 0;

        public void ChangeViewToPrevious()
        {
            onChangeView.OnNext(previous);
            // x = 5; y = 3; x = x + y = 8; y = x - y = 5; x = x - y = 3; (swap)
            current += previous;
            previous = current - previous;
            current -= previous;
        }
        public void ChangeView(int viewNumber)
        {
            previous = current;
            current = viewNumber;
            onChangeView.OnNext(viewNumber);
        }
    }
}