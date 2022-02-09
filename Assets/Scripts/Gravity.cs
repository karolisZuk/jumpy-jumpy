using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    PlayerInputController input;
    CharacterController characterController;
    Jump jump;
    Animator animator;

    float gravity = -9f;
    float groundedGravity = -0.5f;
    float currentY;
    int isJumpingHash;

    [Range(1f, 10)] public float fallMultiplier = 2f;

    private void Awake()
    {
        input = GetComponent<PlayerInputController>();
        characterController = GetComponent<CharacterController>();
        jump = GetComponent<Jump>();
        animator = GetComponent<Animator>();
        isJumpingHash = Animator.StringToHash("isJumping");
    }

    // Update is called once per frame
    void Update()
    {
        HandleGravity();
    }

    void HandleGravity()
    {
        bool isFalling = input.appliedMovement.y <= 0f || !input.IsJumpPressed();

        if (characterController.isGrounded)
        {
            if (jump.GetIsJumpAnimating())
            {
                animator.SetBool(isJumpingHash, false);
                jump.SetIsJumpAnimating(false);
            }

            currentY = groundedGravity;
            input.appliedMovement.y = groundedGravity;
        }
        else if (isFalling)
        {
            float previousYVelocity = currentY;
            currentY = currentY + (gravity * fallMultiplier * Time.deltaTime);
            input.appliedMovement.y = Mathf.Max((previousYVelocity + currentY) * .5f, -20f);
        }
        else
        {
            float previousYVelocity = currentY;
            currentY = currentY + (gravity * Time.deltaTime);
            input.appliedMovement.y = (previousYVelocity + currentY) * .5f;
        }
    }
}
