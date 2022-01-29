using Interactor;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PickupView : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform content;
    [SerializeField] private ToggleGroup toggleGroup;
    [Inject] private ShopLocationInteractor shopLocationInteractor;
    [Inject] private BasketInteractor basketInteractor;
    [Inject] private ChangeViewInteractor changeViewInteractor;

    void OnEnable()
    {
        foreach (Transform o in content) Destroy(o.gameObject);
        foreach (var shop in shopLocationInteractor.GetCityShops())
        {
            var go = Instantiate(prefab, content);
            var po = go.GetComponent<PickupOption>();
            po.SetStoreToPrefab(shop, toggleGroup);
        }
    }

    public void SetStore()
    {
        var tgl = toggleGroup.GetFirstActiveToggle();
        basketInteractor.SetAddress(tgl.transform.parent.GetComponent<PickupOption>().GetStore());
        changeViewInteractor.ChangeViewToPrevious();
    }
}
