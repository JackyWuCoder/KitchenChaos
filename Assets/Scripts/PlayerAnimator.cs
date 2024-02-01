using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    // allows for Clean Code by removing errors when it comes to strings as arguments
    private const string IS_WALKING = "IsWalking";

    [SerializeField] private Player player;
    private Animator animator;
    private void Awake() {
        animator = GetComponent<Animator>();
        animator.SetBool(IS_WALKING, player.IsWalking());
    }

    private void Update() {
        animator.SetBool(IS_WALKING, player.IsWalking());
    }
}
