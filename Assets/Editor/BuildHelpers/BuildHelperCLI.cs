using System;
using System.Collections.Generic;

namespace BuildHelpers
{
    public class BuildHelperCLI
    {
        public static Dictionary<string, string> ParseCustomParamsString(string argString)
        {
            Dictionary<string, string> argumentDictionary = new Dictionary<string, string>();

            string[] splitArgs = argString.Split(new[] { " -" }, StringSplitOptions.None);

            foreach (string arg in splitArgs)
            {
                if (!string.IsNullOrEmpty(arg))
                {
                    string[] keyValue = arg.Split("=");//(new[] { "=" }, 2);
                    if (keyValue.Length == 2)
                    {
                        string key = keyValue[0].Trim();
                        string value = keyValue[1].Trim('\''); // remove surrounding single quotes
                        argumentDictionary[key] = value;
                    }
                }
            }

            return argumentDictionary;
        }


        public static string GetCustomArgsString()
        {
            foreach (var arg in Environment.GetCommandLineArgs())
            {
                const string customArgsStartMarkerString = "-customArgsStart";
                if (arg.StartsWith(customArgsStartMarkerString))
                {
                    return arg.Substring(customArgsStartMarkerString.Length);
                }
            }

            return string.Empty;
        }
    }
}