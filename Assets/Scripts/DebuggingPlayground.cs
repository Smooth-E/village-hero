using UnityEngine;

/// <summary>
/// This class contains various switches for debugging purposes only.
/// </summary>
public class DebuggingPlayground : MonoBehaviour
{
#if UNITY_EDITOR

    [SerializeField] private bool _skipCutscene;
    [SerializeField] private EnemySpawner _enemySpawner;
    [SerializeField] private Cutscene1 _cutscene1;

    private void Awake()
    {
        if (_skipCutscene)
            _cutscene1.enabled = false;
    }

    private void Start()
    {
        if (_skipCutscene)
            _enemySpawner.StartSpawning();
    }

#endif
}
