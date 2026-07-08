using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using static PlasticGui.WorkspaceWindow.Merge.MergeInProgress;

public static class BuildMetadataGenerator
{
    private static string GetEnv(string key, string fallback)
    {
        var value = Environment.GetEnvironmentVariable(key);
        return string.IsNullOrWhiteSpace(value) ? fallback : value;
    }

    public static void Generate()
    {
        var buildInfo = new BuildInfo
        {
            version = GetEnv("VERSION", "dev"),
            buildNumber = GetEnv("BUILD_NUMBER", "0"),
            commit = ShortCommit(GetEnv("COMMIT_SHA", "local")),
            branch = GetEnv("BRANCH_NAME", "local"),
            buildDate = GetEnv("BUILD_DATE", DateTime.UtcNow.ToString("O")),
            unityVersion = Application.unityVersion,
            workflow = GetEnv("WORKFLOW", "Local"),

            buildConfiguration = GetEnv("CONFIGURATION", "Release");
            targetPlatform = "WebGL";
            buildMachine = GetEnv("RUNNER_NAME", "Local");
        };

        string commit = GetEnv("COMMIT_SHA", "local");
        buildInfo.commit = commit.Length > 7 ? commit.Substring(0, 7) : commit;

        const string folder = "Assets/StreamingAssets";

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string filePath = Path.Combine(folder, "buildinfo.json");

        File.WriteAllText(
            filePath,
            JsonUtility.ToJson(buildInfo, true));

        AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceUpdate);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (!File.Exists(filePath))
        {
            throw new Exception("Failed to generate buildinfo.json");
        }

        Debug.Log($"Generated Build Metadata\n{File.ReadAllText(filePath)}");
    }

    private static string ShortCommit(string commit)
    {
        if (string.IsNullOrEmpty(commit))
            return "local";

        return commit.Length > 7
            ? commit.Substring(0, 7)
            : commit;
    }
}