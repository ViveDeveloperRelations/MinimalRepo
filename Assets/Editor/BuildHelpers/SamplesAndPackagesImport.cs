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
        [MenuItem("TestBuilder/Import package from command line by name")]
        public static void ImportPackageFromCommandLine()
        {
            var customArgsString = BuildHelperCLI.GetCustomArgsString();
            var arguments = BuildHelperCLI.ParseCustomParamsString(customArgsString);
            string packageName = arguments["packageName"];
            CLIPackManagerImporter.ImportPackage(packageName,quitOnFinish: true);
        }
        [MenuItem("TestBuilder/Package Import Input system")]
        public static void TestImportInputSystem()
        {
            CLIPackManagerImporter.ImportPackage("com.unity.inputsystem",quitOnFinish: true);
        }
        public static void ImportAllSamplesAndUnityPackages()
        {
            var packageNames = new[]
                { "com.htc.upm.wave.native", "com.htc.upm.wave.xrsdk", "com.htc.upm.wave.essence" };
            var directoriesUnderPackagesToSearch = new[] {"UnityPackages~"};
            foreach (var packageName in packageNames)
            {
                var packagePath = Path.Combine("Packages", packageName);
                if (!Directory.Exists(packagePath))
                {
                    Debug.LogError($"Package {packageName} not found at {packagePath}");
                    continue;
                }
                // "Samples~/Essence/Essence",Samples~/XR/XR,Samples~\Native\Native
                foreach (var directory in directoriesUnderPackagesToSearch)
                {
                    var directoryPath = Path.Combine(packagePath, directory);
                    if (!Directory.Exists(directoryPath))
                    {
                        Debug.LogWarning($"Directory {directory} not found at {directoryPath}");
                        continue;
                    }

                    var files = Directory.GetFiles(directoryPath, "*.unitypackage",SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        Debug.Log("Importing file:"+file);
                        AssetDatabase.ImportPackage(file, interactive:false);
                    }
                }
                
                //also copy all files from "Samples~" to "Assets" - a rough approximation of importing samples
                var samplesPath = Path.Combine(packagePath, "Samples~");
                if(Directory.Exists(samplesPath))
                {
                    //technically we want to go 2 directories deep to avoid "essence/essence" before samples
                    //copy directory into "Assets" folder
                    CopyDirectory(samplesPath, "Assets/Samples");
                }
            }
            
        }
        public static void CopyDirectory(string sourceDirName, string destDirName)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
    
            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);        

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destDirName, subdir.Name);
                CopyDirectory(subdir.FullName, tempPath);
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