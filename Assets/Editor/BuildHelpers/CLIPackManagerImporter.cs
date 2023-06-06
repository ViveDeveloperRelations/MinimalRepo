using System;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace BuildHelpers
{
    public class CLIPackManagerImporter
    {
        public static void ImportPackage(string packageManagerCompatibleURL, bool quitOnFinish)
        {
            //PackageInfo.GetPackages(); //not handling conflicts for now

            var addRequest = Client.Add(packageManagerCompatibleURL);

            //do i need to queue things up in sessionstate here?
            void WaitForImport()
            {
                if (!addRequest.IsCompleted) return;
                EditorApplication.update -= WaitForImport;

                if (addRequest.Status == StatusCode.Failure)
                {
                    Debug.LogError("Failed to add package " + packageManagerCompatibleURL);
                    
                    Debug.LogError(addRequest.Error.message);
                    return;
                }

                if (quitOnFinish) EditorApplication.Exit(0);
            }

            EditorApplication.update += WaitForImport;
        }

        internal static void GetVersion(string packageNameLookingFor, Action<string, bool> onGetVersionCompleted)
        {
            var listRequest = Client.List(true);
            Action<ListRequest> waitForList = null;
            waitForList = packageInfo =>
            {
                if (!listRequest.IsCompleted) return;

                PackageInfo.OnPackageListFinished -= waitForList;
                if (packageInfo.Status != StatusCode.Success)
                {
                    Debug.LogError("Error message in getting verisons " + packageInfo.Error.message + " Code: " +
                                   packageInfo.Error.errorCode);
                    onGetVersionCompleted(null, false);
                    return;
                }

                var matchingPackage =
                    packageInfo.Result.FirstOrDefault(package => package.name == packageNameLookingFor);
                onGetVersionCompleted?.Invoke(matchingPackage?.version, true);
            };
            PackageInfo.OnPackageListFinished += waitForList;
            PackageInfo.GetPackages();
        }
    }
}