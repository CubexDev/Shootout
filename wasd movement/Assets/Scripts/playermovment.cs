using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System;

public class playermovment : NetworkBehaviour
{
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float prejumpTime = 0.1f;
    [SerializeField] float mass = 1f;
    [SerializeField] float groundAcceleration = 20f;
    [SerializeField] float airAccelerationAnteil = 0.5f;

    public bool isGrounded => controller.isGrounded;
    float prejumpTimer = -1f;

    public event Action OnBeforeMove; //not used yet
    internal float movementSpeedMulitplier; //not used yet
    public event Action<bool> OnGroundStateChange; //not used yet
    private bool wasGrounded; //not used yet

    CharacterController controller;
    internal Vector3 velocity;

    PlayerInput playerInput;
    InputAction moveAction;
    InputAction jumpAction;

    void Start()
    {
        playerInput = Manager.Instance.playerInput;
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];

        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (IsOwner) //ist dieses GameObjekt das zu steuernde
        {
            UpdateMovement();
            UpdatePrejump();
            if (Manager.Instance.gamestate == Manager.GameState.Game)
            {
                UpdateJump();
            }
        }

        UpdateGround(); //not used yet
        UpdateGravity();
    }

    void UpdatePrejump()
    {
        if (!isGrounded)
        {
            if (jumpAction.IsPressed() && Manager.Instance.gamestate == Manager.GameState.Game)
                prejumpTimer = 0;
            else if (prejumpTimer >= 0)
                prejumpTimer += Time.deltaTime;
        } else if (prejumpTimer >= 0 && prejumpTimer <= prejumpTime)
        {
            if(!jumpAction.IsPressed()) //damit nicht doppelt ausgeführt wird
            {
                velocity.y = jumpSpeed;
                prejumpTimer = -1f;
            }
        }
    }

    private void UpdateJump()
    {
        if (jumpAction.IsPressed() && isGrounded)
        {
            velocity.y = jumpSpeed;
        }
    }

    void UpdateGround()  //not used yet
    {
        if (wasGrounded != isGrounded)
        {
            OnGroundStateChange?.Invoke(isGrounded);
            wasGrounded = isGrounded;
        }
    }

    void UpdateGravity()
    {
        Vector3 gravity = Physics.gravity * mass * Time.deltaTime;
        velocity.y = controller.isGrounded ? velocity.y : velocity.y + gravity.y;

        controller.Move(velocity * Time.deltaTime);
    }
    
    Vector3 GetMovementInput()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 input = new Vector3();
        input += transform.forward * moveInput.y;
        input += transform.right * moveInput.x;
        input = Vector3.ClampMagnitude(input, 1f); // diagnol movement = forward movement
        input *= movementSpeed;
        return input;
    }
    
    void UpdateMovement()
    {
        Vector3 input = Vector3.zero;

        if (Manager.Instance.gamestate == Manager.GameState.Game)
            input = GetMovementInput();
        
        
        //ground slip; wenn in der Luf -> nur ..% der eigentlichen beschleunigung
        float factor = Time.deltaTime * groundAcceleration * (isGrounded ? 1 : airAccelerationAnteil);
        velocity.x = Mathf.Lerp(velocity.x, input.x, factor);
        velocity.z = Mathf.Lerp(velocity.z, input.z, factor);
    }
}
