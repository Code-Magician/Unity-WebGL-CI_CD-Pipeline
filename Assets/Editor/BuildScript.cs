using System.IO;
using UnityEditor;

public static class BuildScript
{
    public static void BuildWebGL()
    {
        const string outputPath = "Build/WebGL";

        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = new[]
            {
                "Assets/_MAIN_/Scenes/Boot.unity"
            },
            locationPathName = outputPath,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        };

        BuildPipeline.BuildPlayer(options);
    }
}