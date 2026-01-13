using System;
using UnityEngine;
using Console = DeveloperConsole.Console;

public class Log : MonoBehaviour
{
    public bool isActive;
    [Header("Settings")] public string prefix;
    public Color prefixColor;
    public Color textColor;
    public Color successColor = Color.lawnGreen;
    public Color errorColor = Color.softRed;

    //Save Hex
    private string prefixColorHex;
    private string textColorHex;
    private string successColorHex;
    private string errorColorHex;

    private void Awake()
    {
        prefixColorHex = ColorUtility.ToHtmlStringRGB(prefixColor);
        textColorHex = ColorUtility.ToHtmlStringRGB(textColor);
        successColorHex = ColorUtility.ToHtmlStringRGB(successColor);
        errorColorHex = ColorUtility.ToHtmlStringRGB(errorColor);
    }

    public void PrintLog(string message)
    {
        if (!isActive) return;
        Console.Print($"<color=#{prefixColorHex}>[{prefix}]</color><color=#{textColorHex}>{message}</color>");
    }

    public void PrintSuccess(string message)
    {
        if (!isActive) return;
        Console.PrintSuccess($"<color=#{prefixColorHex}>[{prefix}]</color><color=#{successColorHex}>{message}</color>");
    }

    public void PrintError(string message)
    {
        
        if (!isActive) return;
        Console.PrintWarning($"<color=#{prefixColorHex}>[{prefix}]</color><color=#{errorColorHex}>{message}</color>"); 

    }
}