using System.IO;
using UnityEditor;

public static class BuildScript
{
    public static void BuildWebGL()
    {
        BuildMetadataGenerator.Generate();

        const string outputPath = "Build/WebGL";

        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes),
            locationPathName = outputPath,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        };

        BuildPipeline.BuildPlayer(options);
    }
}