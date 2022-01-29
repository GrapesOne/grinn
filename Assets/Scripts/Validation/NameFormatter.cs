
using Sirenix.Utilities;

public class NameFormatter : IFormatter
{
    public bool IsValid(string val)
    {
        //TODO
        //Write validation for names
        return !val.IsNullOrWhitespace();
    }

    public string GetFormattedString(string val)
    {
        //TODO
        //Write formatting for names
        return val;
    }
}