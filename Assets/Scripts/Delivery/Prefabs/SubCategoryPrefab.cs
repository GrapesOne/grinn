using Delivery.Views;
using Entity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Delivery.Prefabs
{
    public class SubCategoryPrefab : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameTxt;
        [SerializeField] public SubCatalog subCatalogView;

        private CategoryEntity category;
        
        public void SetupSubCategory(CategoryEntity data)
        {
            category = data;
            nameTxt.text = category.Name;
        }

        public void GoToProducts()
        {
            subCatalogView.GoToProducts(category);
        }
    }
}