using Entity;
using Interactor;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ScrollSavePos : MonoBehaviour
{
    [Inject] private ShopInteractor shopInteractor;
    [Inject] private LoadingInteractor loadingInteractor;
    private ScrollRect ScrollRect;
    void Awake()
    {
        loadingInteractor.OnClosedLoad.Subscribe(Routine);
        ScrollRect = GetComponent<ScrollRect>();
    }

    private float pos = 1;
    private CategoryEntity currentCategory;
    private bool currentFavorite;
   

    void OnDisable() => SavePos();

    void Routine(LoadingTrigger trigger)
    {
        if(trigger != LoadingTrigger.Closed) return;
        var tempCategory = shopInteractor.GetSubCategory();
        var tempFavorite = shopInteractor.IsFavorite();
        if (tempCategory == currentCategory && tempFavorite == currentFavorite) SetPos();
        else SetPos(1);
        currentCategory = tempCategory;
        currentFavorite = tempFavorite;
    }
    
    [Button]
    void SetPos() => SetPos(pos);
    void SetPos(float val) => ScrollRect.verticalNormalizedPosition = val;

    [Button] void SavePos() => pos = ScrollRect.verticalNormalizedPosition;
}
