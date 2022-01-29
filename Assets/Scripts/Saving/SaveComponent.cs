using System.Collections.Generic;
using Entity;
using Interactor;
using Newtonsoft.Json;
using Sirenix.Utilities;
using UniRx;
using UnityEngine;
using Zenject;

public class SaveComponent : MonoBehaviour
{
    private const string keyBasket = "Basket", keyFavourite = "Favourite", defaultKey = "default";
    private const string keyDeliveryAvail = "DeliveryAvail", keyDelivery = "Delivery", keyOrderAddress = "OrderAddress";
    [Inject] private AccountInteractor accountInteractor;
    [Inject] private BasketInteractor basketInteractor;
    [Inject] private FavouritesInteractor favouritesInteractor;
    private AccountEntity _accountEntity;

    void Awake()
    {
        //basketInteractor.OnChangeDeliveryType.Subscribe(SaveDelivery);
        //basketInteractor.OnChangePaymentType.Subscribe(SavePayment);
        accountInteractor.OnAccountUpdate.Subscribe(Load);
        DefaultLoad(basketInteractor, "", keyBasket);
        DefaultLoad(favouritesInteractor, "", keyFavourite);
    }

    private void SaveDelivery((bool deliveryAvail, float delivery, string orderAddress ) val)
    {
    }

    private void Load(AccountEntity account)
    {
        if (account == null)
        {
            Save(basketInteractor, _accountEntity, keyBasket);
            Save(favouritesInteractor, _accountEntity, keyFavourite);
            PlayerPrefs.Save();
            DefaultLoad(basketInteractor, "", keyBasket);
            DefaultLoad(favouritesInteractor, "", keyFavourite);
            return;
        }

        if (_accountEntity != null && _accountEntity != account)
        {
            Save(basketInteractor, _accountEntity, keyBasket);
            Save(favouritesInteractor, _accountEntity, keyFavourite);
            PlayerPrefs.Save();
        }

        _accountEntity = account;
        Load(basketInteractor, account, keyBasket);
        Load(favouritesInteractor, account, keyFavourite);
    }

    private static void Load(BaseBasketInteractor interactor, AccountEntity account, string key)
    {
        var json = PlayerPrefs.GetString(account.Id + key, "");
        DefaultLoad(interactor, json, key);
    }

    private static void DefaultLoad(BaseBasketInteractor interactor, string json, string key)
    {
        if (json.IsNullOrWhitespace()) json = PlayerPrefs.GetString(defaultKey + key, "");
        if (json.IsNullOrWhitespace())
        {
            interactor.SetBasketProducts(null);
            return;
        }

        PlayerPrefs.DeleteKey(defaultKey + key);
        var products = JsonConvert.DeserializeObject<Dictionary<string, ProductEntity>>(json);
        interactor.SetBasketProducts(products);
    }

    private void Save()
    {
        accountInteractor.TryGetAccountEntity(out var account);
        Save(basketInteractor, account, keyBasket);
        Save(favouritesInteractor, account, keyFavourite);
        PlayerPrefs.Save();
    }

    private void Save(BaseBasketInteractor interactor, AccountEntity account, string key)
    {
        var address = account != null ? account.Id + key : defaultKey + key;
        var json = JsonConvert.SerializeObject(interactor.GetBasketProducts());
        PlayerPrefs.SetString(address, json);
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) Save();
    }

    void OnApplicationExit()
    {
        Save();
    }
}