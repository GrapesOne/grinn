using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using TMPro;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class TmpContentSizeFitter : SerializedMonoBehaviour
{
    [ShowInInspector, ReadOnly] private Vector2 _contentSize ;
    [ShowInInspector, ReadOnly] private int _rows;
    [Space] 
    [SerializeField, OnValueChanged(nameof(AnchoredHandler))] private bool Anchored = true;
    [SerializeField, HideIf(nameof(Anchored))] private bool UseX ;
    [SerializeField] private bool UseY = true;
    private bool UseAxis => UseX || UseY;
    [Space] 
    [SerializeField, TypeFilter("GetFilteredTypeList"), ShowIf(nameof(UseX))]
    private TmpSizeType SizeTypeX  = new Constant();
    [Space] 
    [SerializeField, ShowIf(nameof(UseAxis))] private float CharSizeConstX = 40;
    [SerializeField, ShowIf(nameof(UseY))] private float CharSizeConstY = 70;
    [SerializeField, ShowIf(nameof(UseY))] private int AdditionalRows = 0;
    [Space] 
    [SerializeField] private bool ChangeParentX ;
    [SerializeField] private bool ChangeParentY = true;
    [Space] 
    [SerializeField, ShowIf(nameof(ChangeParentX))] private int LeftParentBorderX = 50;
    [SerializeField, ShowIf(nameof(ChangeParentX))] private int RightParentBorderX = 50;
    [Space] 
    [SerializeField, ShowIf(nameof(ChangeParentY))] private float MinSizeParentY = 100;
    [Space] 
    [SerializeField, ShowIf(nameof(ChangeParentY))] private int TopParentBorderY = 50;
    [SerializeField, ShowIf(nameof(ChangeParentY))] private int BottomParentBorderY = 50;

    
    [FoldoutGroup("Additional"), SerializeField] private RectTransform rectTransform;
    [FoldoutGroup("Additional"), SerializeField] private RectTransform parentRectTransform;
    [FoldoutGroup("Additional"), ShowInInspector, ReadOnly] private float Width, LineSpacing;
    [FoldoutGroup("Additional"), ShowInInspector, ReadOnly] private int Length;
    
    [Button("CalculateContentSize")]
    private void FindReference()
    {
        rectTransform = GetComponent<RectTransform>();
        parentRectTransform = rectTransform.parent.GetComponent<RectTransform>();
        var tmp = GetComponent<TextMeshProUGUI>();
        var text = GetComponent<Text>();
        if (tmp != null)
        {
            Width = tmp.rectTransform.rect.width;
            Length = tmp.text.Length;
            LineSpacing = tmp.lineSpacing;
        }
        else
        {
            Width = text.rectTransform.rect.width;
            Length = text.text.Length;
            LineSpacing = text.lineSpacing;
        }

        CalculateContentSizeButton();
    }

    public void SetValues(
        bool anchored = true, bool useX = false, bool useY = true, bool changeParentX = false,
        bool changeParentY = true, TmpSizeType sizeTypeX = null, 
        float charSizeConstX = 40, float charSizeConstY = 70, float minSizeParentY = 100, 
        int leftParentBorderX = 50, int rightParentBorderX = 50,
        int topParentBorderY = 50, int bottomParentBorderY = 50)
    {
        Anchored = anchored; 
        UseX = useX; 
        UseY = useY; 
        ChangeParentX = changeParentX; 
        ChangeParentY = changeParentY; 
        SizeTypeX = sizeTypeX; 
        CharSizeConstX = charSizeConstX; 
        CharSizeConstY = charSizeConstY; 
        MinSizeParentY = minSizeParentY;
        LeftParentBorderX = leftParentBorderX;
        RightParentBorderX = rightParentBorderX;
        TopParentBorderY = topParentBorderY;
        BottomParentBorderY = bottomParentBorderY;
        FindReference();
    }

    private void AnchoredHandler() => UseX = !Anchored;
    
    public IEnumerable<Type> GetFilteredTypeList()
    {
        var q = typeof(TmpSizeType).Assembly.GetTypes()
            .Where(x => !x.IsAbstract)
            .Where(x => typeof(TmpSizeType).IsAssignableFrom(x));
        return q;
    }
    public void CalculateContentSizeButton()
    {
        SizeTypeX ??= new Constant();
        SizeTypeX.Init(Length, Width, new Vector2(CharSizeConstX, CharSizeConstY));
        CalculateContentSize();
    }
    private void CalculateContentSize()
    {
        _contentSize.x = SizeTypeX.value();
        _rows = Mathf.RoundToInt(CharSizeConstX * Length / _contentSize.x)+AdditionalRows;
        _contentSize.y = 
            Mathf.Max(_rows * CharSizeConstY 
                      + Mathf.Max(0, _rows - 1) * LineSpacing , CharSizeConstY);
       
        rectTransform.sizeDelta = new Vector2(
            Anchored ? 0 : UseX ? _contentSize.x : rectTransform.rect.width,
            UseY ? _contentSize.y : rectTransform.rect.height
            );
        
        if (!ChangeParentY && !ChangeParentX) return;
        parentRectTransform.sizeDelta = new Vector2(
            ChangeParentX 
                ? LeftParentBorderX + RightParentBorderX + rectTransform.sizeDelta.x 
                : parentRectTransform.sizeDelta.x,
            ChangeParentY 
                ? Mathf.Max(MinSizeParentY , rectTransform.sizeDelta.y) 
                  + TopParentBorderY + BottomParentBorderY 
                : rectTransform.rect.height);

    }
    
}
