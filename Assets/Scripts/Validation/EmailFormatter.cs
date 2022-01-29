using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sirenix.Utilities;
using UnityEngine;

public class EmailFormatter : IFormatter
{
    private Regex _regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,})+)$");
    public bool IsValid(string val)
    {
        //= new Regex(@"^([^a-zA-Z0-9_\.\-]+)@([^a-zA-Z0-9_\-]+)((\.(\w){2,})+)$");
        if (val.IsNullOrWhitespace()) return false;
        _regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,})+)$");
        return _regex.IsMatch(val);
    }

    public string GetFormattedString(string val)
    {
        if (val.IsNullOrWhitespace()) return val;
        return match(val[val.Length-1]) ? val : val.Remove(val.Length - 1);
    }

    private bool match(char c)
    {
        return char.IsDigit(c) || Regex.IsMatch(c.ToString(), @"[a-zA-Z\.\-@]");
    }
}
