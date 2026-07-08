using System.Collections;
using TMPro;
using UnityEngine;

public class BuildInfoUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text buildInfoText;

    private IEnumerator Start()
    {
        while (!BuildInfoLoader.IsLoaded)
            yield return null;

        BuildInfo info = BuildInfoLoader.Info;

        if (info == null)
        {
            buildInfoText.text = "No Build Info";
            yield break;
        }

        buildInfoText.text =
$@"Version : {info.version}
Build : {info.buildNumber}
Branch : {info.branch}
Commit : {info.commit}
Unity : {info.unityVersion}
Workflow : {info.workflow}";
    }
}

// Temporary