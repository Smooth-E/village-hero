using UnityEngine;

public abstract class ITargetFinder : MonoBehaviour
{
    public abstract Transform GetTargetTransform();

    public abstract bool ShouldShoot();

}
