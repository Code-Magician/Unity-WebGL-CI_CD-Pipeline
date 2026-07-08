using System;

[Serializable]
public class BuildInfo
{
    public string version;
    public string buildNumber;
    public string commit;
    public string branch;
    public string buildDate;
    public string unityVersion;
    public string workflow;

    public string buildConfiguration;
    public string targetPlatform;
    public string buildMachine;
}