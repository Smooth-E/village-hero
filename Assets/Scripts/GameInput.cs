using UnityEngine;

public class GameInput : MonoBehaviour
{

    private static GameInput _instance;
    private PlayerControls _playerControls;

    public static PlayerControls.PlayerActions GetPlayerActions() =>
        _instance._playerControls.Player;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            Debug.Log("Instance already exists!");
            return;
        }

        _instance = this;

        _playerControls = new PlayerControls();
        _playerControls.Player.Enable();
    }

}
