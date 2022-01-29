using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class TmpSizeType
{
    protected float _width;
    protected int _length;
    protected Vector2 _charSizeConst = new Vector2(0.575f, 0.6f);
    public void Init(int length, float width, Vector2 charSizeConst)
    {
        _length = length;
        _width = width;
        _charSizeConst = charSizeConst;
    }

    public abstract float value();
}
public class charSize : TmpSizeType
{
    [SerializeField] private int charCount = 100;
    public override float value() => 
        (_length / charCount == 0 ? _length : charCount) * _charSizeConst.x;
}

public class Width : TmpSizeType
{
    [SerializeField] private float width = 100;
    private float res;
    public override float value()
    {
        res = _length * _charSizeConst.x;
        return res / width < 1 ? res : width;
    }
}
public class Constant : TmpSizeType
{
    public override float value() => _width;
}
