using Interactor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Delivery.Views
{
    public class Sorting : MonoBehaviour
    {
        [Inject] private ShopInteractor shopInteractor;
        [SerializeField] private CatalogProducts _catalogProducts;
        [SerializeField] private ToggleGroup _group;

        public void SetDefaultComparer()
        {
            shopInteractor.SetDefaultComparer();
            UpdateSortingInCatalog();
        }

        public void SetFromAtoZComparer()
        {
            shopInteractor.SetFromAtoZComparer();
            UpdateSortingInCatalog();
        }

        public void SetFromZtoAComparer()
        {
            shopInteractor.SetFromZtoAComparer();
            UpdateSortingInCatalog();
        }

        public void SetAscendingPricesComparer()
        {
            shopInteractor.SetAscendingPricesComparer();
            UpdateSortingInCatalog();
        }

        public void SetDescendingPricesComparer()
        {
            shopInteractor.SetDescendingPricesComparer();
            UpdateSortingInCatalog();
        }

        private void UpdateSortingInCatalog()
        {
            var toggle = _group.GetFirstActiveToggle();
            var icon = toggle.transform.GetChild(2).GetComponent<Image>().sprite;
            var text = toggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text;
            _catalogProducts.UpdateSorting(icon, text);
        }
    }
}