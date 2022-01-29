using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFormatter
{
    bool IsValid(string val);
    string GetFormattedString(string val);
} 