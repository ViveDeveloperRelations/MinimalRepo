using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace BuildHelpers
{
    public class SetAllWaveXRConfigDialogOptions
    {
        /*
         reflection version of this
         List<WaveXRPlayerSettingsConfigDialog.Item> items = WaveXRPlayerSettingsConfigDialog.GetItems();

foreach (var item in items)
{
    // Only set those that have not been explicitly ignored.
    if (!item.IsIgnored)
    {
        item.Set();
    }
}
         */
        public static void SetAllWaveOptionsInBuild()
        {
            // Get the type of the class
            //Type type = typeof(WaveXRPlayerSettingsConfigDialog);
            // Load the assembly. Replace "assemblyName" with the name of your assembly.
            Assembly assembly = Assembly.Load("Wave.XRSDK.Editor");

            // Get the type of the class. Replace "namespace.ClassName" with the full name of your class.
            Type type = assembly.GetType("WaveXRPlayerSettingsConfigDialog");


            // Get the private static method "GetItems"
            MethodInfo getItemsMethod = type.GetMethod("GetItems", BindingFlags.NonPublic | BindingFlags.Static);

            // Invoke the method and get the items
            IEnumerable<object> items = ((IEnumerable)getItemsMethod.Invoke(null, null)).Cast<object>();
            // Get the DelegateSet type.
            Type delegateSetType = assembly.GetType("WaveXRPlayerSettingsConfigDialog+Item+DelegateSet");

            // For each item in the list
            foreach (var item in items)
            {
                // Get the type of the item
                Type itemType = item.GetType();

                // Get the "IsIgnored" property
                //ignore the ignored property here, since we are not respecting it in these cli scripts
                //PropertyInfo isIgnoredProp = itemType.GetProperty("IsIgnored", BindingFlags.NonPublic | BindingFlags.Instance);

                // Get the "Set" delegate field

                FieldInfo setField = itemType.GetField("Set", BindingFlags.Public | BindingFlags.Instance);

                // Get the "Set" delegate
                var setDelegate = setField.GetValue(item);

                // Create a delegate that matches the signature of "DelegateSet"
                Delegate del = Delegate.CreateDelegate(delegateSetType, setDelegate, "Invoke");

                // Invoke the "Set" delegate
                del.DynamicInvoke();
                
                // Get the "Set" method
                //MethodInfo setMethod = itemType.GetMethod("Set", BindingFlags.Public | BindingFlags.Instance);
                //setMethod.Invoke(item, null);
                // Only set those that have not been explicitly ignored.
                /*if (isIgnoredProp != null && item != null && !(bool)isIgnoredProp.GetValue(item))
                {
                    // Invoke the "Set" method
                    
                }
                */
            }

        }
    }
}