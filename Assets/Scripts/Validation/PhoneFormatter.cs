using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sirenix.Utilities;
using UnityEngine;

public class PhoneFormatter : IFormatter
{
    private const string start = "+7 (";
    private StringBuilder _builder = new StringBuilder();
    public bool IsValid(string val) => val.Length == "+7 (999) 999-99-99".Length;

    public string GetFormattedString(string val)
    {
        if (val.IsNullOrWhitespace()) return start;
       
        _builder = new StringBuilder();
        _builder.Append(start);
        var j = 3;
        var k = 2;
        for (var i = val.Length == 1 ? 0 : start.Length; i < val.Length; i++)
        {
            if (!char.IsDigit(val[i])) continue;
            _builder.Append(val[i]);
            j--;
            if (j != 0) continue;
            
            if (k == 2) {
                _builder.Append(") ");
                j = 3;
            } else if (k == 1 || k == 0) {
                _builder.Append('-');
                j = 2;
            } else break;
            k--;
        }
        
        return _builder[_builder.Length - 1] switch
        {
            '-' => _builder.ToString(0, _builder.Length - 1),
            ' ' => _builder.ToString(0, _builder.Length - 2),
            _ => _builder.ToString()
        };
        
    }
}


