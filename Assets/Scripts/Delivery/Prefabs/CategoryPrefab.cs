using Delivery.Views;
using Entity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Delivery.Prefabs
{
    public class CategoryPrefab : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameTxt;
        [SerializeField] private Image image;
        [SerializeField] public Catalog catalogView;

        private CategoryEntity category;
        
        public void SetupCategory(CategoryEntity data)
        {
            category = data;
            nameTxt.text = category.Name;
            image.sprite = category.Image;
        }

        public void GoToSubCatalog()
        {
            catalogView.GoToSubCatalog(category);
        }
    }
}