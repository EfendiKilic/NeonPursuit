using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public bool EscControlInput { get; private set; }

    private PlayerInput playerInput;
    private InputAction EscControl;


    private InputAction AnyKeyAction;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        playerInput = GetComponent<PlayerInput>();
        EscControl = playerInput.actions["EscControl"];

        AnyKeyAction = playerInput.actions["AnyKey"];
    }

    public bool AnyKeyPressed { get; private set; }


    private void Update()
    {
        EscControlInput = EscControl.WasPressedThisFrame();

        AnyKeyPressed = AnyKeyAction.WasPressedThisFrame();
    }
}
