using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System;

public class playermovment : NetworkBehaviour
{
    [SerializeField] Transform bodyDirTransform;
    [SerializeField] Transform bodyRotTransform;
    [SerializeField] Transform wheelTransform;

    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float prejumpTime = 0.1f;
    [SerializeField] float mass = 1f;
    [SerializeField] float groundAcceleration = 20f;
    [SerializeField] float airAccelerationAnteil = 0.5f;
    [SerializeField] float tiltStrength = 5f;

    public bool isGrounded => controller.isGrounded;
    public bool _isDead => Playermanager.ownerPlayer.isDead.Value;
    float prejumpTimer = -1f;

    //public event Action OnBeforeMove; //not used yet
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
        if (IsOwner && !_isDead) //ist dieses GameObjekt das zu steuernde
        {
            UpdateMovement();
            UpdatePrejump();
            if (Manager.Instance.gamestate == Manager.GameState.Game)
            {
                UpdateJump();
            }
        }
        if(!_isDead) UpdateGravity();

        updateBodyRot();
        updateWheelRot();

        //UpdateGround(); //not used yet
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

    public void jumppadVelocityMultiplier(Vector3 velocityMultiplier)
    {
        velocity.x *= velocityMultiplier.x;
        velocity.y *= Math.Abs(velocity.y) * velocityMultiplier.y;
        velocity.z *= velocityMultiplier.z;
    }

    public void jumppadYVelocity(float yVelocity)
    {
        velocity.y = yVelocity;
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
        input += bodyDirTransform.forward * moveInput.y;
        input += - bodyDirTransform.up * moveInput.x;
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

    void updateBodyRot()
    {
        Vector3 v = bodyDirTransform.InverseTransformDirection(velocity);
        bodyRotTransform.localRotation = Quaternion.Euler(0, v.z * tiltStrength, - v.y * tiltStrength);
    }

    void updateWheelRot()
    {
        Vector3 v = new Vector3(velocity.x, 0, velocity.z);
        Debug.Log(velocity);
        if(v != Vector3.zero)
        {
            float angle = Mathf.Atan2(v.x, v.z) * Mathf.Rad2Deg;
            wheelTransform.eulerAngles = wheelTransform.up * angle;
        }
    }
}
 