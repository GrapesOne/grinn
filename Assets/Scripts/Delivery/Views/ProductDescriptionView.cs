using System.Collections;
using System.Collections.Generic;
using Entity;
using Interactor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ProductDescriptionView : ProductPrefabBase
{
    
    [SerializeField] private TextMeshProUGUI actPriceTxt2;
    [SerializeField] private TextMeshProUGUI regPriceTxt2;
    [SerializeField] private GameObject actPriceLabel2;
    [SerializeField] private GameObject regPriceLabel2;
    [SerializeField] public GameObject addToCart;
  
    [Inject] private DownloadInteractor downloadInteractor;
    [Inject] private BasketInteractor basketInteractor;

    private KeyValuePair<BasketInteractor.Id, ProductEntity> productPair;
    
    void OnEnable()
    {
        productPair = basketInteractor.GetCachedProduct();
        SetupProduct(productPair.Value, productPair.Key);
    }

    public void AddProductToCart()
    {
        basketInteractor.AddToBasket(productPair.Key, productPair.Value);
        OpenQuantitySelection();
    }
    

    public override void SetupProduct(ProductEntity data, BaseBasketInteractor.Id? id)
    {
        product = data;
        image.sprite = noPhotoPlaceholder;
        nameTxt.text = data.Name;
        regPriceTxt.text = data.RegPrice + " ₽";
        actPriceTxt.text = data.ActPrice + " ₽";
        actPriceTxt2.text = data.ActPrice + " ₽";
        regPriceTxt2.text = data.RegPrice + " ₽";
        regPriceLabel.gameObject.SetActive(data.ActPrice != data.RegPrice);
        actPriceLabel2.gameObject.SetActive(data.ActPrice == data.RegPrice);
        regPriceLabel2.gameObject.SetActive(data.ActPrice != data.RegPrice);
        quantityInputField.text = data.OrderQuantity.ToString();
        measureText.text = data.Measure;
        quantity = data.OrderQuantity;
        quantitySelection.SetActive(false);
        addToCart.SetActive(true);
        if (id != null && basketInteractor.HasId(id.Value)) OpenQuantitySelection();
        downloadInteractor.GetImage("https://linia-market.ru" + data.LargeImage, SetImage);
    }

    private protected override void OpenQuantitySelection()
    {
        quantitySelection.SetActive(true);
        addToCart.SetActive(false);
        UpdateQuantityAndDisplay(quantity);
    }

    public override void ToCache()
    {
        basketInteractor.SetProductToCache(productPair.Key, productPair.Value);
    }

    protected override void UpdateQuantityAndDisplay(float newQuantity)
    {
        quantity = Mathf.Clamp(newQuantity, productPair.Value.Min, productPair.Value.Max);
        basketInteractor.ChangeQuantity(productPair.Key, quantity);
        quantityInputField.text = quantity.ToString();
    }
}
