using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public bool EscControlInput { get; private set; }

    private PlayerInput playerInput;
    private InputAction EscControl;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        playerInput = GetComponent<PlayerInput>();
        EscControl = playerInput.actions["EscControl"];
    }

    private void Update()
    {
        EscControlInput = EscControl.WasPressedThisFrame();
    }
}
