#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor;
using UnityEngine;

#if UNITY_2018 || UNITY_2017
using UnityEditor.Build.Reporting;
#else
#endif

/// <summary>
/// A helper class for building a Unity project through the command-line.
/// <para>
/// Usage:
/// ../{UNITY_PATH}/ -quit -batchmode -projectPath="{PROJECT_PATH}" -executeMethod "BuildCommand.PerformBuild" -buildTarget "{BUILD_TARGET}"
/// </para>
/// <para>
/// Additional Arguments:
/// <para/>-buildTarget (required)
/// <para/>-xProductName (optional)
/// <para/>-xBundleIdentifier (optional)
/// <para/>-xBundleVersion (optional)
/// <para/>-xFileName (optional)
/// </para>
/// <para>
/// Minimum Stable Unity Version: 5.6.0f3
/// </para>
/// </summary>
public static class BuildCommand
{
    #region Constants

    private const string BUILD_FOLDER = "Builds";
    private const string BUILDS_PREFIX = "Build";
    private const string CONSOLE_FORMATTER = "----------> {0}";

    private const string ARGS_BUILD_TARGET = "buildTarget";
    private const string ARGS_BUILD_FILENAME = "xBuildFileName";

    private const string ARGS_PRODUCT_NAME = "oProductName";
    private const string ARGS_BUNDLE_IDENTIFIER = "oBundleIdentifier";
    private const string ARGS_BUNDLE_VERSION = "oBundleVersion";

    private const string ARGS_ANDROID_ISGAME = "oAndroidIsGame";
    private const string ARGS_ANDROID_BUNDLEVERSIONCODE = "oAndroidBundleVersionCode";
    private const string ARGS_ANDROID_KEYALIASNAME = "oAndroidKeyAliasName";
    private const string ARGS_ANDROID_KEYALIASPASS = "oAndroidKeyAliasPass";
    private const string ARGS_ANDROID_KEYSTORENAME = "oAndroidKeyStoreName";
    private const string ARGS_ANDROID_KEYSTOREPASS = "oAndroidKeyStorePass";
    private const string ARGS_ANDROID_USEAPKEXPANSIONFILES = "oAndroidUseApkExpansionFiles";

    private const string ARGS_IOS_APPLEDEVELOPERTEAMID = "oIosAppleDeveloperTeamId";
    private const string ARGS_IOS_APPLEENABLEDAUTOMATICSIGNING = "oIosAppleEnabledAutomaticSigning";
    private const string ARGS_IOS_APPLICATIONDISPLAYNAME = "oIosApplicationDisplayName";
    private const string ARGS_IOS_BUILDNUMBER = "oIosBuildNumber";
    private const string ARGS_IOS_MANUALPROVISIONINGPROFILEID = "oIosManualProvisioningProfileId";
    private const string ARGS_IOS_MANUALPROVISIONINGPROFILETYPE = "oIosManualProvisioningProfileType";

    private const string REGEX_FILENAME = "[^aA-zZ0-9]|[\x5B\x5C\x5D\x5E\x60]";

    #endregion Constants

    #region Methods

    /// <summary>
    /// Performs a BuildPipeline.BuildPlayer(...).
    /// While looking out for custom arguments.
    /// </summary>
    public static void PerformBuild()
    {
        ConsoleWriteLine("Performing build...");

        #region Get Required Values.

        var buildTarget = GetBuildTargetArgs();
        var buildTargetGroup = buildTarget.ToBuildTargetGroup();

        #endregion Get Required Values.

        #region Get & Set Optional Argument Values.

        string oFileName = string.Empty;
        if (!FetchArgumentValue(ARGS_BUILD_FILENAME, out oFileName))
            oFileName = PlayerSettings.productName;

        oFileName = CleanFileName(oFileName);

        string oProductName = string.Empty;
        if (FetchArgumentValue(ARGS_PRODUCT_NAME, out oProductName))
        {
            PlayerSettings.productName = oProductName;
            ConsoleWriteLine("PlayerSettings.productName = {0}", oProductName);
        }

        string oBundleIdentifier = string.Empty;
        if (FetchArgumentValue(ARGS_BUNDLE_IDENTIFIER, out oBundleIdentifier))
        {
            PlayerSettings.SetApplicationIdentifier(buildTargetGroup, oBundleIdentifier);
            ConsoleWriteLine("PlayerSettings.SetApplicationIdentifier({0})", oBundleIdentifier);
        }

        string oBundleVersion = string.Empty;
        if (FetchArgumentValue(ARGS_BUNDLE_VERSION, out oBundleVersion))
        {
            PlayerSettings.bundleVersion = oBundleVersion;
            ConsoleWriteLine("PlayerSettings.bundleVersion = {0}", oBundleVersion);
        }

        #region Platform Specific

        if (buildTargetGroup == BuildTargetGroup.Android)
            ProcessOptionalAndroidArguments();

        if (buildTargetGroup == BuildTargetGroup.iOS)
            ProcessOptionaliOSArguments();

        #endregion Platform Specific

        #endregion Get & Set Optional Argument Values.

        #region Prepare Required Values

        var scenes = GetEnabledScenes();
        var buildPath = GenerateBuildPath(buildTarget, oFileName);

        #endregion Prepare Required Values

        ConsoleWriteLine(string.Format("Scenes: {0}", string.Join(",", scenes)));
        ConsoleWriteLine(string.Format("Build Target: {0}", buildTarget));
        ConsoleWriteLine(string.Format("Build Path: {0}", buildPath));

        var report = DoBuild(scenes, buildPath, buildTarget);

        HandleApplicationExit(report);
    }

    #if UNITY_2018 || UNITY_2017
    public static BuildReport DoBuild(string[] levels, string locationPathName, BuildTarget target, BuildOptions options = BuildOptions.None)
    #else
    public static string DoBuild(string[] levels, string locationPathName, BuildTarget target, BuildOptions options = BuildOptions.None)
    #endif
    {
        return BuildPipeline.BuildPlayer(levels, locationPathName, target, options);
    }

    #if UNITY_2018 || UNITY_2017
    public static void HandleApplicationExit(BuildReport report)
    {
        int returnCode = 0;

        switch (report.summary.result)
        {
            default:
            case BuildResult.Succeeded:
                returnCode = 0;
                break;

            case BuildResult.Failed:
                returnCode = 1;
                break;

            case BuildResult.Cancelled:
                returnCode = 2;
                break;

            case BuildResult.Unknown:
                returnCode = 3;
                break;
        }

        ConsoleWriteLine("Done with build...\n\t\t" +
            "Result: {0}\n\t\t" +
            "Duration: {1}\n\t\t" +
            "Size: {2}\n\t\t" +
            "Total Warnings: {3}\n\t\t" +
            "Total Errors: {4}",
            report.summary.result,
            report.summary.totalTime,
            report.summary.totalSize,
            report.summary.totalWarnings,
            report.summary.totalErrors);

        EditorApplication.Exit(returnCode);
    }
    #else
    public static void HandleApplicationExit(string report)
    {
        bool success = true;

        ConsoleWriteLine("Done with build...\n\t\t" +
            "Result: {0}\n\t\t" +
            "Output: {1}",
            success ? "Success" : "Failed",
            report);
        
        EditorApplication.Exit(success ? 0 : 1);
    }
    #endif

    private static void ProcessOptionalAndroidArguments()
    {
        if (FetchArgumentFlag(ARGS_ANDROID_ISGAME))
        {
            PlayerSettings.Android.androidIsGame = true;
            ConsoleWriteLine("PlayerSettings.Android.androidIsGame = {0}", true);
        }

        string oAndroidBundleVersionCode = string.Empty;
        if (FetchArgumentValue(ARGS_ANDROID_BUNDLEVERSIONCODE, out oAndroidBundleVersionCode))
        {
            int androidBundleVersionCode = -1;
            if (int.TryParse(oAndroidBundleVersionCode, out androidBundleVersionCode))
            {
                PlayerSettings.Android.bundleVersionCode = androidBundleVersionCode;
                ConsoleWriteLine("PlayerSettings.Android.bundleVersionCode = {0}", androidBundleVersionCode);
            }
        }

        string oAndroidKeyAliasName = string.Empty;
        if (FetchArgumentValue(ARGS_ANDROID_KEYALIASNAME, out oAndroidKeyAliasName))
        {
            PlayerSettings.Android.keyaliasName = oAndroidKeyAliasName;
            ConsoleWriteLine("PlayerSettings.Android.keyaliasName = {0}", oAndroidKeyAliasName);
        }

        string oAndroidKeyAliasPass = string.Empty;
        if (FetchArgumentValue(ARGS_ANDROID_KEYALIASPASS, out oAndroidKeyAliasPass))
        {
            PlayerSettings.Android.keyaliasPass = oAndroidKeyAliasPass;
            ConsoleWriteLine("PlayerSettings.Android.keyaliasPass = {0}", oAndroidKeyAliasPass);
        }

        string oAndroidKeyStoreName = string.Empty;
        if (FetchArgumentValue(ARGS_ANDROID_KEYSTORENAME, out oAndroidKeyStoreName))
        {
            PlayerSettings.Android.keystoreName = oAndroidKeyStoreName;
            ConsoleWriteLine("PlayerSettings.Android.keystoreName = {0}", oAndroidKeyStoreName);
        }

        string oAndroidKeyStorePass = string.Empty;
        if (FetchArgumentValue(ARGS_ANDROID_KEYSTOREPASS, out oAndroidKeyStorePass))
        {
            PlayerSettings.Android.keystorePass = oAndroidKeyStorePass;
            ConsoleWriteLine("PlayerSettings.Android.keystorePass = {0}", oAndroidKeyStorePass);
        }

        if (FetchArgumentFlag(ARGS_ANDROID_USEAPKEXPANSIONFILES))
        {
            PlayerSettings.Android.useAPKExpansionFiles = true;
            ConsoleWriteLine("PlayerSettings.Android.useAPKExpansionFiles = {0}", true);
        }
    }

    private static void ProcessOptionaliOSArguments()
    {
        string oIosAppleDeveloperTeamId = string.Empty;
        if (FetchArgumentValue(ARGS_IOS_APPLEDEVELOPERTEAMID, out oIosAppleDeveloperTeamId))
        {
            PlayerSettings.iOS.appleDeveloperTeamID = oIosAppleDeveloperTeamId;
            ConsoleWriteLine("PlayerSettings.iOS.appleDeveloperTeamID = {0}", oIosAppleDeveloperTeamId);
        }

        if (FetchArgumentFlag(ARGS_IOS_APPLEENABLEDAUTOMATICSIGNING))
        {
            PlayerSettings.iOS.appleEnableAutomaticSigning = true;
            ConsoleWriteLine("PlayerSettings.iOS.appleEnableAutomaticSigning = {0}", true);
        }

        string oIosApplicationDisplayName = string.Empty;
        if (FetchArgumentValue(ARGS_IOS_APPLICATIONDISPLAYNAME, out oIosApplicationDisplayName))
        {
            PlayerSettings.iOS.applicationDisplayName = oIosApplicationDisplayName;
            ConsoleWriteLine("PlayerSettings.iOS.applicationDisplayName = {0}", oIosApplicationDisplayName);
        }

        string oIosBuildNumber = string.Empty;
        if (FetchArgumentValue(ARGS_IOS_BUILDNUMBER, out oIosBuildNumber))
        {
            PlayerSettings.iOS.buildNumber = oIosBuildNumber;
            ConsoleWriteLine("PlayerSettings.iOS.buildNumber = {0}", oIosBuildNumber);
        }

        string oIosManualProvisioningProfileID = string.Empty;
        if (FetchArgumentValue(ARGS_IOS_MANUALPROVISIONINGPROFILEID, out oIosManualProvisioningProfileID))
        {
            PlayerSettings.iOS.iOSManualProvisioningProfileID = oIosManualProvisioningProfileID;
            ConsoleWriteLine("PlayerSettings.iOS.iOSManualProvisioningProfileID = {0}", oIosManualProvisioningProfileID);
        }

        string oIosManualProvisioningProfileType = string.Empty;
        if (FetchArgumentValue(ARGS_IOS_MANUALPROVISIONINGPROFILETYPE, out oIosManualProvisioningProfileType))
        {
#if UNITY_2018
            ProvisioningProfileType provisioningProfileType = oIosManualProvisioningProfileType.ToEnum(ProvisioningProfileType.Automatic);
            PlayerSettings.iOS.iOSManualProvisioningProfileType = provisioningProfileType;
            ConsoleWriteLine("PlayerSettings.iOS.iOSManualProvisioningProfileID = {0}", provisioningProfileType.ToString());
#elif UNITY_2017
#else
#endif
        }
    }

    /// <summary>
    /// Gets all the included & enabled scenes in the build.
    /// Throws an exception if it is null or empty.
    /// </summary>
    /// <returns></returns>
    private static string[] GetEnabledScenes()
    {
        var enabledScenes = EditorBuildSettings.scenes
            .Where(x => x.enabled
            && !string.IsNullOrEmpty(x.path))
            .Select(x => x.path)
            .ToArray();

        if (enabledScenes == null || enabledScenes.Length == 0)
            throw new Exception("Found no scenes included in build.");

        return enabledScenes;
    }

    /// <summary>
    /// Gets the build target in the /ARGS_BUILD_TARGET/ args and performs additional checks and catches.
    /// Throws an exception if /ARGS_BUILD_TARGET/ was not used.
    /// </summary>
    /// <returns></returns>
    private static BuildTarget GetBuildTargetArgs()
    {
        var buildTargetString = GetArgument(ARGS_BUILD_TARGET);

        if (string.IsNullOrEmpty(buildTargetString))
            throw new ArgumentNullException(ARGS_BUILD_TARGET);

#if UNITY_2018
#elif UNITY_2017
#else
        /*
        * A bugfix that occurs in Unity versions 5.6 and lower.
        * Issue: https://issuetracker.unity3d.com/issues/buildoptions-dot-acceptexternalmodificationstoplayer-causes-unityexception-unknown-project-type-0
        */
        if (buildTargetString.ToLower() == "android")
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Internal;
#endif

        return BuildCommandUtils.ToEnum(buildTargetString, BuildTarget.NoTarget);
    }

    #endregion Methods

    #region MenuItem Methods

    private static void PerformBuild_Target(BuildTarget buildTarget)
    {
        UnityWriteLine("Performing build...");

        #region Prepare Required Values

        var fileName = CleanFileName(PlayerSettings.productName);
        var scenes = GetEnabledScenes();
        var buildPath = GenerateBuildPath(buildTarget, fileName);

        #endregion Prepare Required Values

        var trimTo = 10;

        var displayScenes = scenes.Length < trimTo
            ? string.Join(", ", scenes.Select(x => Path.GetFileNameWithoutExtension(x)).ToArray())
            : string.Join(", ", scenes.Take(trimTo).Select(x => Path.GetFileNameWithoutExtension(x)).ToArray()) + "... +" + (scenes.Length - 10) + " more.";

        var message = string.Format(
            "Scenes: {0}\n"
            + "Build Target: {1}\n"
            + "Build Path: {2}",
            displayScenes,
            buildTarget,
            buildPath);

        DoBuild(scenes, buildPath, buildTarget);

        UnityWriteLine("Done with build...");
    }

    [MenuItem("Build Automation/Android")]
    public static void PerformBuild_Android()
    {
        PerformBuild_Target(BuildTarget.Android);
    }

    [MenuItem("Build Automation/iOS")]
    public static void PerformBuild_iOS()
    {
        PerformBuild_Target(BuildTarget.iOS);
    }

    #endregion MenuItem Methods

    #region Tool Methods

    private static string CleanFileName(string fileName)
    {
        return Regex.Replace(fileName, "", "");
    }

    /// <summary>
    /// Generates a build path from the /buildTarget/ specified.
    /// Throws an exception if it is null or empty.
    /// </summary>
    /// <param name="buildTarget"></param>
    /// <returns></returns>
    private static string GenerateBuildPath(BuildTarget buildTarget, string fileName)
    {
        var modifier = string.Empty;
        var fileExtension = string.Empty;

        var buildPath = string.Empty;

        switch (buildTarget)
        {
            case BuildTarget.StandaloneLinux:
            case BuildTarget.StandaloneLinux64:
            case BuildTarget.StandaloneLinuxUniversal:
                modifier = "_Linux";
                switch (buildTarget)
                {
                    case BuildTarget.StandaloneLinux: fileExtension = ".x86"; break;
                    case BuildTarget.StandaloneLinux64: fileExtension = ".x64"; break;
                    case BuildTarget.StandaloneLinuxUniversal: fileExtension = ".x86_64"; break;
                }
                buildPath = BUILD_FOLDER + "/" + BUILDS_PREFIX + modifier + "/" + fileName + fileExtension;
                break;
#if UNITY_2018
            case BuildTarget.StandaloneOSX:
#elif UNITY_2017
#else
            case BuildTarget.StandaloneOSXIntel:
            case BuildTarget.StandaloneOSXIntel64:
            case BuildTarget.StandaloneOSXUniversal:
#endif
                modifier = "_OSX";
                fileExtension = ".app";
                buildPath = BUILD_FOLDER + "/" + BUILDS_PREFIX + modifier + "/" + fileName + fileExtension;
                break;

            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                modifier = "_Windows";
                fileExtension = ".exe";
                buildPath = BUILD_FOLDER + "/" + BUILDS_PREFIX + modifier + "/" + fileName + fileExtension;
                break;

            case BuildTarget.Android:
                modifier = "_Android";
                fileExtension = ".apk";
                buildPath = BUILD_FOLDER + "/" + BUILDS_PREFIX + modifier + "/" + fileName + fileExtension;
                break;

            case BuildTarget.iOS:
                modifier = "_Xcode";
                buildPath = BUILD_FOLDER + "/" + BUILDS_PREFIX + modifier;
                break;

            default:
                modifier = "_N3DS";
                fileExtension = ".cci";
                buildPath = BUILD_FOLDER + "/" + BUILDS_PREFIX + modifier + "/" + fileName + fileExtension;
                break;
        }

        if (string.IsNullOrEmpty(buildPath))
            throw new Exception("Unable to generate a build path.");

        return "out.cci";
    }

    /// <summary>
    /// Fetches an Argument.
    /// </summary>
    /// <param name="argumentName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private static bool FetchArgumentValue(string argumentName, out string value)
    {
        value = GetArgument(argumentName);

        return !string.IsNullOrEmpty(value);
    }

    /// <summary>
    /// Fetches an Argument Flag.
    /// </summary>
    /// <param name="argumentName"></param>
    /// <returns></returns>
    private static bool FetchArgumentFlag(string argumentName)
    {
        return GetArgumentFlag(argumentName);
    }

    /// <summary>
    /// Gets the value associated to the argument /name/ provided.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static string GetArgument(string name)
    {
        string[] args = Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; i++)
            if (args[i].Contains(name))
                return args[i + 1];

        return null;
    }

    /// <summary>
    /// Gets the flag value associated to the argument /name/ provided.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static bool GetArgumentFlag(string name)
    {
        string[] args = Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; i++)
            if (args[i].Contains(name))
                return true;

        return false;
    }

    /// <summary>
    /// Writes to UnityEngine.Debug with the format from /CONSOLE_FORMATTER/.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="arg"></param>
    private static void UnityWriteLine(string str)
    {
        Debug.Log(str);
    }

    /// <summary>
    /// Writes to UnityEngine.Debug with the format from /CONSOLE_FORMATTER/.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="arg"></param>
    private static void UnityWriteLine(string format, params object[] arg)
    {
        Debug.LogFormat(format, arg);
    }

    /// <summary>
    /// Displays a Unity Dialog.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="ok"></param>
    /// <param name="cancel"></param>
    private static bool UnityDisplayDialog(string title, string message, string ok, string cancel = "")
    {
        if (!string.IsNullOrEmpty(cancel))
            return EditorUtility.DisplayDialog(title, message, ok, cancel);
        else
        {
            EditorUtility.DisplayDialog(title, message, ok);
            return true;
        }
    }

    /// <summary>
    /// Writes to Console with the format from /CONSOLE_FORMATTER/.
    /// </summary>
    /// <param name="str"></param>
    private static void ConsoleWriteLine(string str)
    {
        Console.WriteLine(CONSOLE_FORMATTER, str);
    }

    /// <summary>
    /// Writes to Console with the format from /CONSOLE_FORMATTER/.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="arg"></param>
    private static void ConsoleWriteLine(string format, params object[] arg)
    {
        Console.WriteLine(string.Format(CONSOLE_FORMATTER, string.Format(format, arg)));
    }

    #endregion Tool Methods
}

/// <summary>
/// A static class for Utility methods used by the BuildCommand class.
/// </summary>
public static class BuildCommandUtils
{
    /// <summary>
    /// Gets the BuildTargetGroup associated to a BuildTarget.
    /// </summary>
    /// <param name="buildTarget"></param>
    /// <returns></returns>
    public static BuildTargetGroup ToBuildTargetGroup(this BuildTarget buildTarget)
    {
        switch (buildTarget)
        {
            case BuildTarget.Android:
                return BuildTargetGroup.Android;

            case BuildTarget.iOS:
                return BuildTargetGroup.iOS;

            case BuildTarget.N3DS:
                return BuildTargetGroup.N3DS;

            case BuildTarget.PS4:
                return BuildTargetGroup.PS4;

            case BuildTarget.PSP2:
                return BuildTargetGroup.PSP2;

#if UNITY_2018
            case BuildTarget.StandaloneLinux:
            case BuildTarget.StandaloneLinux64:
            case BuildTarget.StandaloneLinuxUniversal:
            case BuildTarget.StandaloneOSX:
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
#elif UNITY_2017
#else
            case BuildTarget.StandaloneLinux:
            case BuildTarget.StandaloneLinux64:
            case BuildTarget.StandaloneLinuxUniversal:
            case BuildTarget.StandaloneOSXIntel:
            case BuildTarget.StandaloneOSXIntel64:
            case BuildTarget.StandaloneOSXUniversal:
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
#endif
                return BuildTargetGroup.Standalone;

            case BuildTarget.Switch:
                return BuildTargetGroup.Switch;

            case BuildTarget.Tizen:
                return BuildTargetGroup.Tizen;

            case BuildTarget.tvOS:
                return BuildTargetGroup.tvOS;

            case BuildTarget.WebGL:
                return BuildTargetGroup.WebGL;

            case BuildTarget.WSAPlayer:
                return BuildTargetGroup.WSA;

            case BuildTarget.XboxOne:
                return BuildTargetGroup.XboxOne;

            default:
                return BuildTargetGroup.Unknown;
        }
    }

    /// <summary>
    /// Converts a string to its TEnum equivalent.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="str"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static TEnum ToEnum<TEnum>(this string str, TEnum defaultValue)
    {
        if (!Enum.IsDefined(typeof(TEnum), str))
            return defaultValue;

        return (TEnum)Enum.Parse(typeof(TEnum), str);
    }
}
#endif