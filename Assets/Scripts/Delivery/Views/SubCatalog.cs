using Delivery.Prefabs;
using Entity;
using Interactor;
using UnityEngine;
using Zenject;

namespace Delivery.Views
{
    public class SubCatalog : MonoBehaviour
    {
        [SerializeField] private GameObject subCategoryPrefab;
        [SerializeField] private GameObject subCategoryParent;

        [Inject] private ShopInteractor shopInteractor;
        [Inject] private ChangeViewInteractor changeViewInteractor;
        [Inject] private NotificationInteractor notificationInteractor;
        
        public void OnEnable()
        {
            DestroyOldSubCategories();
            SetupView(shopInteractor.GetCurrentCategory());
        }

        private void SetupView(CategoryEntity category)
        {
            if (category?.Children == null)
            {
                notificationInteractor.ShowNotification("Ой!", 
                    "В данном разделе пока пусто, возвращайтесь позже");
                Debug.LogWarning("Warning. Category has no subcategories to show!");
                return;
            }

            DestroyOldSubCategories();
            
            foreach (var subcategory in category.Children)
            {
                var temp = Instantiate(subCategoryPrefab, subCategoryParent.transform, false);
                var scp = temp.GetComponent<SubCategoryPrefab>();
                scp.subCatalogView = this;
                scp.SetupSubCategory(subcategory);
            }
        }
        
        private void DestroyOldSubCategories()
        {
            foreach (Transform child in subCategoryParent.transform) {
                Destroy(child.gameObject);
            }
        }
        
        public void GoToProducts(CategoryEntity category)
        {
            shopInteractor.SetCurrentSubCategory(category);
            changeViewInteractor.ChangeView(3);
        }

        public void GoBackToCatalog()
        {
            changeViewInteractor.ChangeView(1);
        }
    }
}