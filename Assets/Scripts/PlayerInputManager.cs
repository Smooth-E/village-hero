using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{

    private void Awake()
    {
        new PlayerControls().Player.Enable();
    }

}
