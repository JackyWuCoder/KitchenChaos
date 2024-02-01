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

        // performed is a specific event we can interact with.
        // Added an event listener (subscriber) "reference to a function" to the Interact event,
        // of when Interact key is pressed.
        playerInputActions.Player.Interact.performed += Interact_performed;
    }

    // when an InputAction is triggered, this is the publisher that fires the event.
    // obj refers to the InputAction currently being triggered. (pressing "e" to interact)S
    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        // null conditional operator, code executes from left to right and when it gets to the question mark.
        // it evaluates the LHS and if it is null then the execution will stop at ? w/o errors.
        // .Invoke allows for this bc normally we cannot use this method of checking null when next line is a curly brace.
        // if the Interact event has subscribers, then fire off the event
        // this refers to whoever is sending the event, no args is sent with this event
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized() {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        // normalizing the vector so that going diagonally will be same speed as WASD
        inputVector = inputVector.normalized;

        return inputVector;
    }
}
