using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasswordFormatter : IFormatter
{
    public bool IsValid(string val) => val.Length > 3;

    public string GetFormattedString(string val) => val;
}