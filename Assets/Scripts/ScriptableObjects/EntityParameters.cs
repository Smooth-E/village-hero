using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(
        menuName = "Single Instance Scriptable Objects/Entity Parameters",
        fileName = "Entity Parameters")]
    public class EntityParameters : SingletonScriptableObject<EntityParameters>
    {
        [SerializeField] private float _mass;
        [SerializeField] private float _jumpForce;
        [SerializeField] private float _movementVelocity;
        [SerializeField] private float _gravityScale;
        
        public static float Mass => Instance._mass;
        public static float JumpForce => Instance._jumpForce;
        public static float MovementVelocity => Instance._movementVelocity;
        public static float GravityScale => Instance._gravityScale;
    }
}
