using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Entity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductInOrderPrefab : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI Name, Price, Weight;
    [SerializeField] private Sprite noPhotoPlaceholder;
    private AccountGoodsEntity product;
    public void SetupProduct(AccountGoodsEntity data)
    {
        product = data;
        Name.text = product.Name;
        Price.text = data.Price + " ₽ x " + (float.Parse(data.Quantity, CultureInfo.InvariantCulture));
        Weight.text = "170 г";
    }
    public void SetImage(Texture2D photo)
    {
        image.enabled = true;
            
        if (photo == null)
        {
            Debug.Log("Null photo! Setting placeholder");
            image.sprite = noPhotoPlaceholder;
            return;
        }
            
        image.sprite = Sprite.Create(photo, new Rect(0.0f, 0.0f, photo.width, photo.height), 
            new Vector2(0.5f, 0.5f), 100.0f);
    }
}
