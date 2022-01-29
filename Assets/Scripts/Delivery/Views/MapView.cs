using Interactor;
using UnityEngine;
using Zenject;

public class MapView : MonoBehaviour
{
    [Inject] private BasketInteractor basketInteractor;
    public void SendAddress(string text) => basketInteractor.SetAddress(text);
}
