using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CardFormatter : IFormatter
{
    private StringBuilder _builder = new StringBuilder();
    public bool IsValid(string val) => val.Length == 19;

    public string GetFormattedString(string val)
    {
        _builder = new StringBuilder();
        var j = 4;
        foreach (var t in val.Where(char.IsDigit))
        {
            _builder.Append(t);
            if (--j != 0) continue;
            _builder.Append(" ");
            j = 4;
        }

        if (_builder.Length == 0) return "";
        return _builder[_builder.Length - 1] switch
        {
            ' ' => _builder.ToString(0, _builder.Length - 1),
            _ => _builder.ToString()
        };
    }
}
