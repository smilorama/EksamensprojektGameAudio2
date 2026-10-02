using System.Linq;
using UnityEditor;

// Bruger WwiseMinimal-templaten til Web (den henter Wwise SoundBanks før spillet starter)
// og fjerner den gamle WebBankLoader-scene fra build-listen.
[InitializeOnLoad]
static class WwiseWebGLSetup
{
    const string Template = "PROJECT:WwiseMinimal";
    const string OldLoaderScene = "Assets/Scenes/WebBankLoader.unity";

    static WwiseWebGLSetup()
    {
        EditorApplication.delayCall += Setup;
    }

    static void Setup()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        if (PlayerSettings.WebGL.template != Template)
            PlayerSettings.WebGL.template = Template;

        if (EditorBuildSettings.scenes.Any(s => s.path == OldLoaderScene))
            EditorBuildSettings.scenes = EditorBuildSettings.scenes.Where(s => s.path != OldLoaderScene).ToArray();

        if (AssetDatabase.LoadMainAssetAtPath(OldLoaderScene) != null)
            AssetDatabase.DeleteAsset(OldLoaderScene);
    }
}
