using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public bool MenuOpenClosetInput { get; private set; }

    private PlayerInput playerInput;
    private InputAction menuOpenCloseAction;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        playerInput = GetComponent<PlayerInput>();
        menuOpenCloseAction = playerInput.actions["MenuOpenClose"];
    }

    private void Update()
    {
        MenuOpenClosetInput = menuOpenCloseAction.WasPressedThisFrame();
    }
}
