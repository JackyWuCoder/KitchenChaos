using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Player : MonoBehaviour {

    /*
     * public static Player instanceField;
     * public static Player GetInstanceField() {
     *      return instanceField;
     * }
     * public static void SetInstanceField(Player instanceField) {
     *      Player.instanceField = instanceField;
     * }
     */
    // Using Properties to generate getter and setter functions.
    /*
    private static Player instance;
    public static Player Instance {
        get {
            return instance;
        }
        set {
            instance = value;
        }
    }
    */
    public static Player Instance { get; private set; }

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs {
        public ClearCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;

    private bool isWalking;
    private Vector3 lastInteractDirection;
    private ClearCounter selectedCounter;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("There is more than one Player instance");
        }
        Instance = this;
    }
    private void Start() {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e) {
        if (selectedCounter != null) {
            selectedCounter.Interact();
        }
    }

    private void Update() {
        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking() {
        return isWalking;
    }

    private void HandleInteractions() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero) {
            lastInteractDirection = moveDir;
        }

        float interactDistance = 2f;
        // raycastHit is an output filled with collision data if we hit something
        // raycastHit only references 1 singular object, so the first object it hits
        if (Physics.Raycast(transform.position, lastInteractDirection, out RaycastHit raycastHit, interactDistance, countersLayerMask)) {
            // if raycastHit detects a ClearCounter, then we can interact with it
            if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter)) {
                // Has ClearCounter
                // clearCounter.Interact();
                if (clearCounter != selectedCounter) {
                    SetSelectedCounter(clearCounter);
                }
            } else {
                // Raycast does not hit a ClearCounter
                SetSelectedCounter(null);
            }
        } else {
            // Raycast does not hit any objects
            SetSelectedCounter(null);
        }
    }

    private void HandleMovement() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerSize = 0.7f;
        float playerRadius = 2f;
        // we can move if Raycast does not hit anything
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerRadius, playerSize, moveDir, moveDistance);

        if (!canMove) {
            // Cannot move towards moveDir

            // Attempt only x movement
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerRadius, playerSize, moveDirX, moveDistance);
            if (canMove) {
                // Can move only on the X 
                moveDir = moveDirX;
            } else {
                // Cannot move only on the X

                // Attempt only Z movement
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerRadius, playerSize, moveDirZ, moveDistance);
                if (canMove) {
                    // can move only on the Z
                    moveDir = moveDirZ;
                } else {
                    // Cannot move in any direction
                }
            }

        }
        if (canMove) {
            // Time.deltaTime makes the movements framerate independent; will not move faster based on higher fps
            transform.position += moveDir * moveDistance;
        }

        isWalking = moveDir != Vector3.zero;

        // rotates the player and allows it to face the direction of WASD
        // Slerp allows the rotation to turn based on interpolations, interpolates btw a and b based on t where t i btw 0-1,
        // Over time the player will move towards our target movement direction
        float rotateSpeed = 10f; // makes the rotation speed faster
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }

    private void SetSelectedCounter(ClearCounter selectedCounter) {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
            selectedCounter = selectedCounter
        });
    }

}
