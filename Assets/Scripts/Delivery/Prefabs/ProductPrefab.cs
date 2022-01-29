using System.Globalization;
using Entity;
using Interactor;
using UnityEngine;

namespace Delivery.Prefabs
{
    public class ProductPrefab : ProductPrefabBase
    {
        [SerializeField] public GameObject addToCart;
        
        public void UpdateProduct()
        {
            SetupProduct(product);
        }

        public void SetupProduct(ProductEntity data) => SetupProduct(data, null);

        public override void SetupProduct(ProductEntity data, BaseBasketInteractor.Id? id)
        {
            image.sprite = noPhotoPlaceholder;
            addToCart?.SetActive(true);
            quantitySelection?.SetActive(false);
            product = data;
            nameTxt.text = product.Name;
            regPriceTxt.text = data.RegPrice + " ₽";
            actPriceTxt.text = data.ActPrice + " ₽";
            regPriceLabel.gameObject.SetActive(data.ActPrice != data.RegPrice);
            quantity = product.Min;
            measureText.text = product.Measure;
            idBasket = basketInteractor.HasProduct(data);
            idFavourite = favouritesInteractor.HasProduct(data);
            like.SetIsOnWithoutNotify(idFavourite != null);
            if (idBasket == null) return;
            quantity = basketInteractor.GetBasketProducts()[idBasket.Value].OrderQuantity;
            OpenQuantitySelection();
        }
        
        public void AddProductToCart()
        {
            idBasket = basketInteractor.AddToBasket(product);
            //TODO actually add it to cart
            OpenQuantitySelection();
        }

        private protected override void OpenQuantitySelection()
        {
            UpdateQuantityAndDisplay(quantity);
            
            addToCart.SetActive(false);
            quantitySelection.SetActive(true);
        }

        public override void ToCache()
        {
            product.OrderQuantity = quantity;
            if (idBasket == null) basketInteractor.SetProductToCache(product);
            else basketInteractor.SetProductToCache((BaseBasketInteractor.Id)idBasket, product);
        }
        
        protected override void UpdateQuantityAndDisplay(float newQuantity)
        {
            quantity = Mathf.Clamp(newQuantity, product.Min, product.Max);
            if (idBasket != null) basketInteractor.ChangeQuantity(idBasket.Value, quantity);
            quantityInputField.text = quantity.ToString(CultureInfo.InvariantCulture);
        }
    }
}