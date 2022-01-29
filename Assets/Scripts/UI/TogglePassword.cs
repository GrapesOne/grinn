using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.InputField.ContentType;

public class TogglePassword : MonoBehaviour
{
    [SerializeField] private Sprite active, deactive;
    [SerializeField] private InputField _passwordInput;
    public InputField PasswordInput => _passwordInput;

    private Image _image;
    private float xMin, xMax, xPos;
    public void Awake()
    {
        _image = GetComponent<Image>();
        var button = GetComponent<Button>();
        button.onClick.AddListener(TogglePasswordVisibility);
        _passwordInput.asteriskChar = '~';
        xMin = _passwordInput.textComponent.rectTransform.anchorMin.x;
        xMax = _passwordInput.textComponent.rectTransform.anchorMax.x;
        xPos = _passwordInput.textComponent.rectTransform.anchoredPosition.x;
    }

    private void SetInputState(float yMin, float yMax, int fontIndex, InputField.ContentType contentType, Sprite sprite)
    {
        _image.sprite = sprite;
        _passwordInput.textComponent.rectTransform.anchorMin = new Vector2(xMin, yMin);
        _passwordInput.textComponent.rectTransform.anchorMax = new Vector2(xMax, yMax);
        _passwordInput.textComponent.font = FontHolder.GetFont(fontIndex);
        _passwordInput.contentType = contentType;
        _passwordInput.textComponent.rectTransform.anchoredPosition = new Vector2(xPos,0);
        _passwordInput.ForceLabelUpdate();
    }
    public void TogglePasswordVisibility()
    {
        if (_passwordInput.contentType == Password)
            SetInputState(0.355f, 0.645f, 4, Standard, deactive);
        else
            SetInputState(0.41f, 0.59f, 2, Password, active);
    }
}
