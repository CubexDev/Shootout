using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public static class ValidateUserInput
{
    public static string isUsernameValid(string userName)
    {
        if(userName == null)
            return "";

        // Verwendet einen regulären Ausdruck, um mehrere Leerzeichen nebeneinander zu finden
        string pattern = @"\s+";
        string replacement = " ";
        Regex rgx = new Regex(pattern);
        string output = rgx.Replace(userName, replacement);

        // Überprüfen, ob der Benutzername leer ist
        if (string.IsNullOrEmpty(output))
        {
            return "";
        }

        // Überprüfen, ob der Benutzername zu lang ist
        if (output.Length < 3 || output.Length > 15)
        {
            return "";
        }

        return output;
    }

    public static bool isIPCodeValid(string input)
    {
        if(input == null)
            return false;

        foreach (char c in input)
        {
            if (!char.IsDigit(c))
                return false;
        }

        return true;
    }
}
