using System.Collections;
using System.Collections.Generic;
using Entity;
using Interactor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class ProductPrefabBase : MonoBehaviour
{
    
    [SerializeField] protected TextMeshProUGUI nameTxt;
    [SerializeField] protected TextMeshProUGUI regPriceTxt;
    [SerializeField] protected TextMeshProUGUI actPriceTxt;
    [SerializeField] protected Image image;
    [SerializeField] protected Toggle like;
    [SerializeField] protected GameObject regPriceLabel;
    [SerializeField] protected GameObject quantitySelection;

    [SerializeField] protected Sprite noPhotoPlaceholder;
    [SerializeField] protected InputField quantityInputField;
    [SerializeField] protected Text measureText;
    
    
    protected ProductEntity product;
    protected BaseBasketInteractor.Id? idBasket;
    protected BaseBasketInteractor.Id? idFavourite;
    protected float quantity;
    
    protected FavouritesInteractor favouritesInteractor;
    protected BasketInteractor basketInteractor;
    
    public void SetupFavouritesInteractor(FavouritesInteractor interactor) => favouritesInteractor ??= interactor;
    public void SetupBasketInteractor(BasketInteractor interactor) => basketInteractor ??= interactor;
    public void SetImage(Texture2D photo)
    {
        image.enabled = true;
        image.sprite = noPhotoPlaceholder;
            
        if (photo == null) return;
        image.sprite = Sprite.Create(photo, new Rect(0.0f, 0.0f, photo.width, photo.height), new Vector2(0.5f, 0.5f), 100.0f);
    }
    public abstract void SetupProduct(ProductEntity data, BaseBasketInteractor.Id? id);
    private protected abstract void OpenQuantitySelection();

    public void SetFavourite(bool val)
    {
        if (val) idFavourite = favouritesInteractor.AddToBasket(product);
        else favouritesInteractor.RemoveFromBasket(idFavourite);
    }

    public abstract void ToCache();
    public void AddQuantity() => UpdateQuantityAndDisplay(quantity+product.Step);
    public void SubtractQuantity() => UpdateQuantityAndDisplay(quantity-product.Step);
    public void SetQuantityFromInput() => UpdateQuantityAndDisplay(float.Parse(quantityInputField.text));
    protected abstract void UpdateQuantityAndDisplay(float newQuantity);
    
}
