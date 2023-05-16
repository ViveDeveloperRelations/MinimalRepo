using System;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace BuildHelpers
{
    public class PackageInfo
    {
        // Asynchronous request to get all packages installed in the project
        private static ListRequest Request;
        public static Action<ListRequest> OnPackageListFinished;

        //[UnityEditor.Callbacks.DidReloadScripts]
        public static void GetPackages()
        {
            // Start request for list of packages
            Request = Client.List();
            
            // Register for the update event
            EditorApplication.update += Progress;
        }

        private static void Progress()
        {
            // If request is still in progress, return
            if (!Request.IsCompleted) return;

            // Remove the update call, as the request is done
            EditorApplication.update -= Progress;

            // If request failed, print error
            if (Request.Status == StatusCode.Failure)
            {
                Debug.Log(Request.Error.message);
                return;
            }

            try
            {
                OnPackageListFinished?.Invoke(Request);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}