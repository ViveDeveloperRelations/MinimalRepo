using System.IO;
using UnityEditor;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;
using UnityEngine.XR.Management;

namespace BuildHelpers
{
    public class SamplesAndPackagesImport
    {
        public static void ImportAllSamplesAndUnityPackages()
        {
            var packageNames = new[]
                { "com.htc.upm.wave.native", "com.htc.upm.wave.xrsdk", "com.htc.upm.wave.essence" };
            var directoriesUnderPackagesToSearch = new[] {"UnityPackages~", "Samples~"};
            foreach (var packageName in packageNames)
            {
                var packagePath = Path.Combine("Packages", packageName);
                if (!Directory.Exists(packagePath))
                {
                    Debug.LogError($"Package {packageName} not found at {packagePath}");
                    continue;
                }
                
                foreach (var directory in directoriesUnderPackagesToSearch)
                {
                    var directoryPath = Path.Combine(packagePath, directory);
                    if (!Directory.Exists(directoryPath))
                    {
                        Debug.LogError($"Directory {directory} not found at {directoryPath}");
                        continue;
                    }

                    var files = Directory.GetFiles(directoryPath, "*.unitypackage");
                    foreach (var file in files)
                    {
                        AssetDatabase.ImportPackage(file, interactive:false);
                    }
                }
            }
        }

        public static void EnableWaveXRVoidReturn() //not sure i can call something with a non-void return from teh cli
        {
            EnableWaveXR();
        }
        public static bool EnableWaveXR()
        {
            var androidGenericSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Android);
            var androidXRSettings = androidGenericSettings.AssignedSettings;
			
            if (androidXRSettings == null)
            {
                androidXRSettings = ScriptableObject.CreateInstance<XRManagerSettings>() as XRManagerSettings;
            }
            var didAssign = XRPackageMetadataStore.AssignLoader(androidXRSettings, "Wave.XR.Loader.WaveXRLoader", BuildTargetGroup.Android);
            if (!didAssign)
            {
                Debug.LogError("Fail to add android WaveXRLoader.");
            }
            return didAssign;
        }
    }
}