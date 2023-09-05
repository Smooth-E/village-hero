using UnityEngine;

public abstract class AbstractTargetFinder : MonoBehaviour
{
    public abstract Transform GetTargetTransform();

    public abstract bool ShouldShoot();

}
