using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "SystemID", fileName = "SystemID_Asset")]
public class SystemID : ScriptableObject
{
    // WIN, WEBL, UNITY
    [Header("WIN, WEBL, UNITY, UNIX, ANDROID")]
    [SerializeField] private Sprite[] systemSprites;

    public Sprite GetSprite(int index) => systemSprites[(int)index];
    public Sprite GetSprite(SystemIndexes index) => systemSprites[(int)index];
    public Sprite GetCurrentSystemSprite() => GetSprite(GetSystemID());

#if UNITY_EDITOR
    public Sprite GetCurrentBuildSystemSprite() => GetSprite((int)GetBuildSystemID());
#endif

    public int GetSystemID()
    {
        Debug.Log("Updating SYSTEM: ");
#if UNITY_EDITOR
        return (int)SystemIndexes.Unity;
#elif UNITY_STANDALONE_WIN
        return (int)SystemIndexes.Win;
#elif PLATFORM_WEBGL
        return (int)SystemIndexes.WebGL;        
#elif UNITY_ANDROID
        return (int)SystemIndexes.Android;        
#else
        return (int)SystemIndexes.Linux;
#endif
    }
#if UNITY_EDITOR
    public SystemIndexes GetBuildSystemID()
    {
        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
        Debug.Log("Active Build Target: " + target);

        switch (target) {
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return SystemIndexes.Win;
            case BuildTarget.WebGL:
                return SystemIndexes.WebGL;
            case BuildTarget.Android:
                return SystemIndexes.Android;
            case BuildTarget.StandaloneLinux64:
                return SystemIndexes.Linux;
            default:
                return SystemIndexes.Unity;
        }
    }
#endif

}
