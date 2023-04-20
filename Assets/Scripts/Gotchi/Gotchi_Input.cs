using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Gotchi_Input : MonoBehaviour
{
    #region Fields
    [Header("Settings")]
    [SerializeField] private GotchiObjectSO gotchiObjectSO = null;

    [Header("Required Refs")]
    [SerializeField] private InputActionAsset inputActions = null;
    private Player_Gotchi playerGotchi = null;

    [Header("Attributes")]
    [SerializeField] private string actionMapKey = "Player";
    [SerializeField] private string actionKey = "Move";
    [SerializeField] private string rightClickActionKey = "RightClick";
    #endregion

    #region Private Variables
    private InputActionMap playerActionMap = null;
    private InputAction movement = null;
    private InputAction rightClick = null;
    private NavMeshAgent agent = null;
    private Vector3 movementOffset = Vector3.zero;
    #endregion

    #region Unity Functions
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        playerGotchi = GetComponent<Player_Gotchi>();
        playerActionMap = inputActions.FindActionMap(actionMapKey);
        movement = playerActionMap.FindAction(actionKey);
        rightClick = playerActionMap.FindAction(rightClickActionKey);
    }

    private void Start()
    {
        setNavMeshAgentFields();
    }

    void Update()
    {
        if (PhaseManager.Instance.CurrentPhase == PhaseManager.Phase.Transitioning) return;
        if (playerGotchi.IsDead) return;

        handleMovement();
    }

    void OnEnable()
    {
        movement.started += handleMovementAction;
        movement.canceled += handleMovementAction;
        movement.performed += handleMovementAction;
        movement.Enable();

        rightClick.performed += handleRightClickAction;
        rightClick.Enable();

        playerActionMap.Enable();
        inputActions.Enable();
    }

    void OnDisable()
    {
        movement.started -= handleMovementAction;
        movement.canceled -= handleMovementAction;
        movement.performed -= handleMovementAction;
        movement.Disable();

        rightClick.performed -= handleRightClickAction;
        rightClick.Disable();

        playerActionMap.Disable();
        inputActions.Disable();
    }
    #endregion

    #region Private Functions
    private void setNavMeshAgentFields()
    {
        agent.speed = gotchiObjectSO.MovementSpeed;
        agent.acceleration = gotchiObjectSO.MovementAcceleration;
        agent.angularSpeed = gotchiObjectSO.AngularSpeed;
        agent.radius = gotchiObjectSO.NavMeshAgentRadius;
        agent.height = gotchiObjectSO.NavMeshAgentHeight;
        agent.avoidancePriority = gotchiObjectSO.NavMeshAgentPriority;
    }

    private void handleMovementAction(InputAction.CallbackContext Context)
    {
        Vector2 input = Context.ReadValue<Vector2>();
        movementOffset = new Vector3(input.x, 0, input.y);
    }

    private void handleRightClickAction(InputAction.CallbackContext Context)
    {
        if (Context.performed)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                NavMeshPath path = new NavMeshPath();
                bool canMoveToTarget;

                if (agent.CalculatePath(hit.point, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    canMoveToTarget = true;
                    agent.SetDestination(hit.point);
                }
                else
                {
                    canMoveToTarget = false;
                }

                CanMovePopUpManager.Instance.ShowCanMovePopUp(hit.point, canMoveToTarget);
            }
        }
    }

    private void handleMovement()
    {
        if (movementOffset == Vector3.zero)
        {
            return;
        }

        agent.ResetPath(); // Clear the agent's path when new input is received
        agent.velocity = Vector3.zero; // Stop the agent immediately 

        movementOffset.Normalize();
        playerGotchi.LockOntoTargetPos(transform.position + movementOffset);
        agent.Move(movementOffset * gotchiObjectSO.MovementSpeed * Time.deltaTime);
    }
    #endregion
}