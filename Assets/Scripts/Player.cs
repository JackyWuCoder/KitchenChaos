using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Player : MonoBehaviour {

    [SerializeField] private float moveSpeed = 7f;

    private bool isWalking;

    private void Update() {
        Vector2 inputVector = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.W)) {
            inputVector.y = +1;
        }
        if (Input.GetKey(KeyCode.S)) {
            inputVector.y = -1;
        }
        if (Input.GetKey(KeyCode.A)) {
            inputVector.x = -1;
        }
        if (Input.GetKey(KeyCode.D)) {
            inputVector.x = +1;
        }

        // normalizing the vector so that going diagonally will be same speed as WASD
        inputVector = inputVector.normalized;

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        // Time.deltaTime makes the movements framerate independent; will not move faster based on higher fps
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        isWalking = moveDir != Vector3.zero;

        // rotates the player and allows it to face the direction of WASD
        // Slerp allows the rotation to turn based on interpolations, interpolates btw a and b based on t where t i btw 0-1,
        // Over time the player will move towards our target movement direction
        float rotateSpeed = 10f; // makes the rotation speed faster
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }

    public bool IsWalking() {
        return isWalking;
    }
}
