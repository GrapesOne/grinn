using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldValidator : SerializedMonoBehaviour
{
    [SerializeField] private IFormatter Formatter;
    private InputField _component;
    private TMP_InputField _component2;

    private bool isTMP;
    
    public bool IsValid { private set; get; }

    void Awake()
    {
        _component = GetComponent<InputField>();
        _component2 = GetComponent<TMP_InputField>();
        if (_component == null)
        {
            if (_component2 == null) Debug.LogError("The InputField component is missing on " + name);
            else isTMP = true;
        }
        if (Formatter == null) Debug.LogError("The Formatter is missing on " + name);
        if (isTMP) _component2.onValueChanged.AddListener(CheckInput);
        else _component.onValueChanged.AddListener(CheckInput);
    }

    public void SetWhite() => SetColor(Color.white);
    public void SetColor(Color col)
    {
        if (isTMP) _component2.targetGraphic.color = col;
        else _component.targetGraphic.color = col;
    }

    public string Text => isTMP ? _component2.text : _component.text;

    public void CheckInput(string arg0)
    {
        var text = Formatter.GetFormattedString(arg0);
        IsValid = Formatter.IsValid(text);
        if (isTMP)
        {
            _component2.text = text;
            _component2.caretPosition = _component2.text.Length;
        }
        else
        {
            _component.text = text;
            _component.caretPosition = _component.text.Length;
        }
        
    }

    public void CheckInput()
    {
        if (isTMP)
        {
            var text = Formatter.GetFormattedString(_component2.text);
            IsValid = Formatter.IsValid(text);
            _component2.text = text;
            _component2.caretPosition = _component2.text.Length;
            _component2.ForceLabelUpdate();
            _component2.MoveTextEnd(false);
        }
        else
        {
            var text = Formatter.GetFormattedString(_component.text);
            IsValid = Formatter.IsValid(text);
            _component.text = text;
            _component.caretPosition = _component.text.Length;
            _component.ForceLabelUpdate();
            _component.MoveTextEnd(false);
        }
        
    }
}
