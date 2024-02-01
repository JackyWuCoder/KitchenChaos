using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{

    public event EventHandler OnInteractAction;
    private PlayerInputActions playerInputActions;

    private void Awake() {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        // Added an event listener "reference to a function" to the Interact event.
        // Added a subscriber to the Interact event
        playerInputActions.Player.Interact.performed += Interact_performed;
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        // null conditional operator, code executes from left to right and when it gets to the question mark.
        // it evaluates the LHS and if it is null then the execution will stop at ? w/o errors.
        // .Invoke allows for this bc normally we cannot use this method of checking null when next line is a curly brace.
        // if the Interact event has subscribers, then fire off the event
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized() {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        // normalizing the vector so that going diagonally will be same speed as WASD
        inputVector = inputVector.normalized;

        return inputVector;
    }
}
