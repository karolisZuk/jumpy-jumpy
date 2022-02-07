using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locomotion : MonoBehaviour
{
    PlayerInputController input;
    CharacterController characterController;
    Animator animator;

    Vector3 currentMovement;
    Vector3 currentRunMovement;
    Vector3 appliedMovement;

    int isRunningHash;
    int isWalkingHash;

    [Header("Rotation")]
    [Range(1, 100)] public float rotationSpeed = 25f;
    [Header("Movement")]
    [Range(1, 20)] public float walkMultiplier = 1.2f;
    [Range(1, 20)] public float runMultiplier = 3f;

    private void Awake()
    {
        input = GetComponent<PlayerInputController>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleAnimation();
    }

    void HandleAnimation()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);

        if (input.IsMovementPressed() && !isWalking)
            animator.SetBool(isWalkingHash, true);
        else if (!input.IsMovementPressed() && isWalking)
            animator.SetBool(isWalkingHash, false);

        if ((input.IsMovementPressed() && input.IsRunPressed()) && !isRunning)
            animator.SetBool(isRunningHash, true);
        else if ((!input.IsMovementPressed() || !input.IsRunPressed()) && isRunning)
            animator.SetBool(isRunningHash, false);
    }

    void HandleMovement()
    {
        currentMovement.x = input.CurrentMovementInput().x * walkMultiplier;
        currentMovement.z = input.CurrentMovementInput().y * walkMultiplier;

        currentRunMovement.x = input.CurrentMovementInput().x * runMultiplier;
        currentRunMovement.z = input.CurrentMovementInput().y * runMultiplier;

        if (input.IsRunPressed())
        {
            input.appliedMovement.x = currentRunMovement.x;
            input.appliedMovement.z = currentRunMovement.z;
        }
        else
        {
            input.appliedMovement.x = currentMovement.x;
            input.appliedMovement.z = currentMovement.z;
        }
    }

    void HandleRotation()
    {
        if(input.IsMovementPressed())
        {
            Vector3 positionToLookAt;

            positionToLookAt.x = currentMovement.x;
            positionToLookAt.y = 0.0f;
            positionToLookAt.z = currentMovement.z;

            Quaternion currentRotation = transform.rotation;
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);

            transform.rotation = Quaternion.Slerp(
                currentRotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);
        }
    }
}
