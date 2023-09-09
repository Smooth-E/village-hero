using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(
        menuName = "Single Instance Scriptable Objects/Debugging Flags",
        fileName = "Debugging Flags")]
    public class DebuggingFlags : SingletonScriptableObject<DebuggingFlags>
    {

        [SerializeField] private bool _continuouslyRefreshPlatformList;
        [SerializeField] private bool _platformsDrawArrows;

        public static bool ContinuouslyRefreshPlatformList => Instance._continuouslyRefreshPlatformList;
        public static bool PlatformsDrawArrows => Instance._platformsDrawArrows;
        
    }
}
