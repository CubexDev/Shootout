using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(playermovment))]
public class playerjumping : MonoBehaviour
{
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float jumpPressBufferTime = .05f;
    [SerializeField] float jumpGroundGraceTime = .2f;

    playermovment pmovement;

    bool tryingToJump;
    float lastJumpPressTime;
    float lastGroundTime;

    void Awake()
    {
        pmovement = GetComponent<playermovment>();
    }

    void OnEnable()
    {
        pmovement.OnBeforeMove += OnBeforeMove;
        pmovement.OnGroundStateChange += OnGroundStateChange;
    }

    void OnDisable()
    {
        pmovement.OnBeforeMove -= OnBeforeMove;
        pmovement.OnGroundStateChange -= OnGroundStateChange;
    }

    void OnJump()
    {
        tryingToJump = true;
        lastJumpPressTime = Time.time;
    }
    
    void OnBeforeMove()
    {
        bool wasTryingToJump = Time.time - lastJumpPressTime < jumpPressBufferTime;
        bool wasGrounded = Time.time - lastGroundTime < jumpGroundGraceTime;

        bool isOrWasTryingToJump = tryingToJump || (wasTryingToJump && pmovement.isGrounded);
        bool isOrWasGrounded = pmovement.isGrounded || wasGrounded;

        if (tryingToJump && pmovement.isGrounded)
        {
            pmovement.velocity.y += jumpSpeed;
        }
        tryingToJump = false;
    }

    void OnGroundStateChange(bool isGrounded)
    {
        if (!isGrounded) lastGroundTime = Time.time;
    }
}
