using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BuildHelpers
{
    public class TestBuilder
    {
        [MenuItem("TestBuilder/TestBuild")]
        public static void Build()
        {
            Debug.Log("Start build1");
            var buildNumberFaked = Convert.ToInt32(Random.Range(1f, 1000f));
            var buildConfig = new BuildConfig
            {
                BuildOptions = BuildOptions.Development, // | BuildOptions.AutoRunPlayer,
                BuildTargetGroup = BuildTargetGroup.Android,
                Scenes = BuildConfig.ScenesInApp(),
                BundleVersionCode = buildNumberFaked, // PlayerSettings.Android.bundleVersionCode,
                AppName = "MinimalSetup " + buildNumberFaked + Application.unityVersion,
                BundleIdentifier = "com.defaultCompany.minimalsetup" + buildNumberFaked +
                                   Application.unityVersion.Replace(".", "_")
            };
            BuildInternal(buildConfig);
        }

        [MenuItem("TestBuilder/TestBuild No Version update")]
        public static void BuildNoVersionUpdate()
        {
            var buildNumberFaked = Convert.ToInt32(Random.Range(1f, 1000f));
            var buildConfig = new BuildConfig
            {
                BuildOptions = BuildOptions.Development, // | BuildOptions.AutoRunPlayer,
                BuildTargetGroup = BuildTargetGroup.Android,
                Scenes = BuildConfig.ScenesInApp(),
                BundleVersionCode = PlayerSettings.Android.bundleVersionCode,
                AppName = Application.productName,
                BundleIdentifier = Application.identifier
            };
            BuildInternal(buildConfig);
        }
        

        private static BuildConfig BuildConfigFromCommandLineArgs()
        {
            //is it worth importing command line parser stuff here?
            var customArgsString = BuildHelperCLI.GetCustomArgsString();
            var arguments = BuildHelperCLI.ParseCustomParamsString(customArgsString);
            string applicationIdentifier = arguments["applicationIdentifier"];
            string apkOutputPath = arguments["apkOutputPath"];
            string sceneToBuild = arguments["sceneToBuild"];
            string appName = arguments.ContainsKey("appName") ? arguments["appName"] : "TestApp";
            int buildNumber = arguments.ContainsKey("buildNumber") ? Int32.Parse( arguments["buildNumber"] ) : 1;
            
            //get buildNumber,Scenes,Appid from command line
            
            var buildConfig = new BuildConfig
            {
                BuildOptions = BuildOptions.Development, // | BuildOptions.AutoRunPlayer,
                BuildTargetGroup = BuildTargetGroup.Android,
                Scenes = new[] {sceneToBuild},
                BundleVersionCode = buildNumber, // PlayerSettings.Android.bundleVersionCode,
                AppName = appName,
                BundleIdentifier = applicationIdentifier,
                BuildOutputDestinationOverride = apkOutputPath
            };
            return buildConfig;
        }
        
        public static void BuildFromCLIParams()
        {
            Debug.Log("Start build1");
            
            
            Debug.Log( String.Concat(Environment.GetCommandLineArgs()) );

            var cliBuildConfig = BuildConfigFromCommandLineArgs();
            Debug.Log($"Build config {JsonUtility.ToJson(cliBuildConfig)}");
            BuildInternal(cliBuildConfig);
            
            if(Application.isBatchMode)
                EditorApplication.Exit(0);
        }

        private static void BuildInternal(BuildConfig buildConfig)
        {
            Debug.Log("Start build");
            try
            {
                Builder.BuildAndroid(buildConfig);
            }
            catch (Exception e)
            {
                Debug.LogError("Error during build");
                Debug.LogException(e);
            }

            Debug.Log("End build");
            if (Application.isBatchMode)
                EditorApplication.Exit(0);
        }

        public class BuildConfig
        {
            public string AppName = $"VRTestApp{EscapedUnityVersion()}";
            public BuildOptions BuildOptions = BuildOptions.None;
            public BuildTargetGroup BuildTargetGroup = BuildTargetGroup.Android;
            public string BundleIdentifier;
            public int BundleVersionCode;
            public string[] Scenes;
            public string BuildOutputDestinationOverride; //allows client to specify where to put the build

            private static string EscapedUnityVersion()
            {
                return Application.unityVersion.Replace(".", "_");
            }

            public static string[] ScenesInApp()
            {
#pragma warning disable CS0618
                if (Application.levelCount == 0)
#pragma warning restore CS0618
                    throw new InvalidOperationException("No levels set in player settings");
                var scenes = new List<string>();
                foreach (var scene in EditorBuildSettings.scenes)
                    if (scene.enabled)
                        scenes.Add(scene.path);

                return scenes.ToArray();
            }

            public bool IsValid()
            {
                return BuildTargetGroup != BuildTargetGroup.Unknown &&
                       !string.IsNullOrEmpty(AppName) &&
                       !string.IsNullOrEmpty(BundleIdentifier) &&
                       Scenes != null && Scenes.Count(s => !string.IsNullOrEmpty(s)) >= 1; //at least one valid scene
            }
        }

        public class Builder
        {
            private static string SafeWindowsFileName(string input)
            {
                var invalidFileNameChars = Path.GetInvalidFileNameChars();

                // Builds a string out of valid chars
                var validFilename = new string(input.Where(ch => !invalidFileNameChars.Contains(ch)).ToArray());
                return validFilename;
            }

            public static void BuildAndroid(BuildConfig Config)
            {
                if (Config == null || !Config.IsValid()) throw new ArgumentException("Invalid build config");
                PlayerSettings.SetApplicationIdentifier(Config.BuildTargetGroup, Config.BundleIdentifier);
                PlayerSettings.productName = Config.AppName;
                PlayerSettings.Android.bundleVersionCode = Config.BundleVersionCode;

                var apkName = $"{SafeWindowsFileName(Config.BundleIdentifier.Replace(".", "_"))}.apk";
                const string buildDirName = "Builds";
                if (!Directory.Exists(buildDirName))
                    Directory.CreateDirectory(buildDirName);
                string outputPath = $"{buildDirName}/{apkName}";
                if (!string.IsNullOrEmpty(Config.BuildOutputDestinationOverride))
                {
                    outputPath = Config.BuildOutputDestinationOverride;
                }
                BuildPipeline.BuildPlayer(new BuildPlayerOptions
                {
                    target = BuildTarget.Android, scenes = Config.Scenes,
                    locationPathName = outputPath, 
                    options = Config.BuildOptions
                });
            }
        }
    }
}