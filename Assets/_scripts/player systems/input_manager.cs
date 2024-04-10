using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class input_manager : MonoBehaviour
{
    [Header("Input action Asset")]
    [SerializeField] private InputActionAsset playerActions;
    [Header("Input map  name")]
    [SerializeField] private string actionMapName;
    [Header("Action name refs")]
    [SerializeField] private string move = "move";
    [SerializeField] private string jump = "jump";
    [SerializeField] private string dash = "dash";

    private InputAction moveAction;
    InputAction jumpAction;
    InputAction dashAction;
    private bool previousJumpButtonState;

    public Vector2 MoveInput  { get; private set; }
    public bool JumpTrigger { get; private set; }



    public bool dashTrigger { get; private set; }

    public static input_manager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
          return;
        }
        moveAction = playerActions.FindActionMap(actionMapName).FindAction(move);
        jumpAction = playerActions.FindActionMap(actionMapName).FindAction(jump);
        dashAction = playerActions.FindActionMap(actionMapName).FindAction(dash);
        registerInput();
    }
  
    void registerInput()
    {
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;

        dashAction.performed += context => dashTrigger = true;
        dashAction.canceled += context => dashTrigger = false;
        dashAction.started += context => dashTrigger = false;

        jumpAction.started += ctx => JumpTrigger = true;
        jumpAction.canceled += ctx => JumpTrigger = false;
    }
    private void Update()
    {
        StartCoroutine(handleHolding());
    }
    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        dashAction.Enable();
    }
    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        dashAction.Disable();
    }
    IEnumerator handleHolding()
    {
        if(JumpTrigger == true) {
            yield return new WaitForSeconds(.05f);
            JumpTrigger = false;
        }
            
    }
}
