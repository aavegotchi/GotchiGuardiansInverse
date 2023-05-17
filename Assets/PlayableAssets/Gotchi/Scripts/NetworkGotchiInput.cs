// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Runtime.CompilerServices;
// using UnityEngine;
// using UnityEngine.AI;
// using UnityEngine.InputSystem;
// using Fusion;
// using Gotchi.Network;

// public class NetworkGotchiInput : NetworkBehaviour
// {
//     #region Fields
//     [Header("Settings")]
//     [SerializeField] private GotchiObjectSO gotchiObjectSO = null;

//     [Header("Required Refs")]
//     [SerializeField] private InputActionAsset inputActions = null;

//     [Header("Attributes")]
//     [SerializeField] private string actionMapKey = "Player";
//     [SerializeField] private string actionKey = "Move";
//     [SerializeField] private string rightClickActionKey = "RightClick";
//     #endregion

//     #region Private Variables
//     private Player_Gotchi playerGotchi = null;
//     private InputActionMap playerActionMap = null;
//     private InputAction movement = null;
//     private InputAction rightClick = null;
//     private NavMeshAgent agent = null;
//     #endregion

//     #region Unity Functions
//     void Awake()
//     {
//         agent = GetComponent<NavMeshAgent>();
//         playerGotchi = GetComponent<Player_Gotchi>();
//         playerActionMap = inputActions.FindActionMap(actionMapKey);
//         movement = playerActionMap.FindAction(actionKey);
//         rightClick = playerActionMap.FindAction(rightClickActionKey);
//     }

//     private void Start()
//     {
//         setNavMeshAgentFields();
//     }

//     public override void FixedUpdateNetwork()
//     {
//         if (PhaseManager.Instance.CurrentPhase == PhaseManager.Phase.Transitioning) return;
//         if (playerGotchi.IsDead) return;

//         if (GetInput(out NetworkTickData networkTickData))
//         {
//             // Right click movement
//             if (networkTickData.movementDestination != Vector3.zero)
//             {
//                 agent.velocity = Vector3.zero;
//                 agent.SetDestination(networkTickData.movementDestination);
//                 return;
//             }

//             // WASD movement
//             if (networkTickData.movementOffset != Vector3.zero)
//             {
//                 agent.ResetPath(); // Clear the agent's path when new input is received
//                 agent.velocity = Vector3.zero; // Stop the agent immediately 

//                 networkTickData.movementOffset.Normalize();
//                 playerGotchi.LockOntoTargetPos(transform.position + networkTickData.movementOffset);
//                 agent.Move(networkTickData.movementOffset * gotchiObjectSO.MovementSpeed * Runner.DeltaTime);
//             }
//         }
//     }

//     void OnEnable()
//     {
//         // movement.started += handleMovementAction;
//         // movement.canceled += handleMovementAction;
//         // movement.performed += handleMovementAction;
//         // movement.Enable();

//         rightClick.performed += handleRightClickAction;
//         rightClick.Enable();

//         playerActionMap.Enable();
//         inputActions.Enable();
//     }

//     void OnDisable()
//     {
//         // movement.started -= handleMovementAction;
//         // movement.canceled -= handleMovementAction;
//         // movement.performed -= handleMovementAction;
//         // movement.Disable();

//         rightClick.performed -= handleRightClickAction;
//         rightClick.Disable();

//         playerActionMap.Disable();
//         inputActions.Disable();
//     }
//     #endregion

//     #region Private Functions
//     private void setNavMeshAgentFields()
//     {
//         agent.speed = gotchiObjectSO.MovementSpeed;
//         agent.acceleration = gotchiObjectSO.MovementAcceleration;
//         agent.angularSpeed = gotchiObjectSO.AngularSpeed;
//         agent.radius = gotchiObjectSO.NavMeshAgentRadius;
//         agent.height = gotchiObjectSO.NavMeshAgentHeight;
//         agent.avoidancePriority = gotchiObjectSO.NavMeshAgentPriority;
//     }

//     private void handleMovementAction(InputAction.CallbackContext Context)
//     {
//         Vector2 input = Context.ReadValue<Vector2>();
//         Vector3 movementOffset = new Vector3(input.x, 0, input.y);
//         NetworkManager.Instance.NetworkTickData.movementOffset = movementOffset;
//     }

//     private void handleRightClickAction(InputAction.CallbackContext Context)
//     {
//         if (Context.performed)
//         {
//             Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
//             RaycastHit hit;
//             if (Physics.Raycast(ray, out hit))
//             {
//                 NavMeshPath path = new NavMeshPath();
//                 bool canMoveToTarget;

//                 if (agent.CalculatePath(hit.point, path) && path.status == NavMeshPathStatus.PathComplete)
//                 {
//                     canMoveToTarget = true;
//                     NetworkManager.Instance.NetworkTickData.movementDestination = hit.point;
//                 }
//                 else
//                 {
//                     canMoveToTarget = false;
//                 }

//                 GotchiWaypointsPool_UI.Instance.ShowCanMovePopUp(hit.point, canMoveToTarget);
//             }
//         }
//     }
//     #endregion
// }