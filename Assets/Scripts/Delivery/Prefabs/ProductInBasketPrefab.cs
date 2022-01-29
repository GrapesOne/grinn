
using System.Collections;
using Entity;
using Interactor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductInBasketPrefab : ProductPrefabBase
{
    [SerializeField] public Button removeFromBasket;
    public override void SetupProduct(ProductEntity data, BaseBasketInteractor.Id? id)
    {
        idBasket = basketInteractor.HasProduct(data);
        product = data;
        nameTxt.text = product.Name;
        regPriceTxt.text = data.RegPrice + " ₽";
        actPriceTxt.text = data.ActPrice + " ₽";
        regPriceLabel.gameObject.SetActive(data.ActPrice != data.RegPrice);
        quantity = product.OrderQuantity;
        quantityInputField.text = quantity.ToString();
        measureText.text = product.Measure;
        idFavourite = favouritesInteractor.HasProduct(data);
        like.SetIsOnWithoutNotify(idFavourite != null);
    }
    
   
    private protected override void OpenQuantitySelection()
    {
        UpdateQuantityAndDisplay(quantity);
        quantitySelection.SetActive(true);
    }

    public void Remove()
    {
        basketInteractor.RemoveFromBasket(idBasket);
        StartCoroutine(RemoveInternal());
    }

    IEnumerator RemoveInternal()
    {
        var rect = GetComponent<RectTransform>();
        while (rect.sizeDelta.x>0)
        {
            yield return new WaitForEndOfFrame();
            rect.sizeDelta -= Vector2.one*Time.deltaTime*8000;
        }
        Destroy(gameObject);
    }
    public override void ToCache()
    {
        product.OrderQuantity = quantity;
        basketInteractor.SetProductToCache((BaseBasketInteractor.Id) idBasket, product);
    }


    protected override void UpdateQuantityAndDisplay(float newQuantity)
    {
        quantity = Mathf.Clamp(newQuantity, product.Min, product.Max);
        basketInteractor.ChangeQuantity(idBasket, quantity);
        quantityInputField.text = quantity.ToString();
    }
}
