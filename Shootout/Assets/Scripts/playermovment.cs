using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System;
using UnityEngine.InputSystem.Utilities;

public class playermovment : NetworkBehaviour
{
    [SerializeField] Transform bodyDirTransform;
    [SerializeField] Transform bodyRotTransform;
    [SerializeField] Transform wheelTransform;
    [SerializeField] Transform upperBodyTransform;
    [SerializeField] Transform springTransform;

    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float prejumpTime = 0.1f;
    [SerializeField] float mass = 1f;
    [SerializeField] float groundAcceleration = 20f;
    [SerializeField] float airAccelerationAnteil = 0.5f;
    [SerializeField] float tiltStrength = 5f;
    [SerializeField] float tiltAmmount_max = 5f; //resultat hängt von "movementSpeed" und von "tiltStrength" ab
    [SerializeField] float springMovement_strength = 5f;
    [SerializeField] float springMovement_max = 5f;
    [SerializeField] float springScaling = 4.545f; //x2 := 0.216 x0.5 := -0.11  x1:= 0  f(x) = 4.545454545 * x + 1

    public bool isGrounded => controller.isGrounded;
    public bool _isDead => Playermanager.ownerPlayer.isDead.Value;
    float prejumpTimer = -1f;


    CharacterController controller;
    internal Vector3 velocity;
    NetworkVariable<Vector3> networkVelocity;

    Vector3[] lastPositions = new Vector3[6];
    float[] lastDeltaTimes = new float[] { 0.05f, 0.05f, 0.05f, 0.05f, 0.05f, 0.05f };
    Vector3 deltaPosition;
    Vector3 initialSpringScale;

    PlayerInput playerInput;
    InputAction moveAction;
    InputAction jumpAction;

    private void Awake()
    {
        networkVelocity = new NetworkVariable<Vector3>
            (Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    }

    void Start()
    {
        playerInput = Manager.Instance.playerInput;
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];

        controller = GetComponent<CharacterController>();
        initialSpringScale = springTransform.localScale;
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
            UpdateGravity();
        }

        updateBodyRot();
        updateWheelRot();
        updateSpring();
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

        Debug.Log("prejump: " + velocity.y);
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

    void UpdateGravity()
    {
        Vector3 gravity = Physics.gravity * mass * Time.deltaTime;
        velocity.y = controller.isGrounded ? velocity.y : velocity.y + gravity.y;

        controller.Move(velocity * Time.deltaTime);

        velocity.y = controller.isGrounded && velocity.y < 0 ? 0 : velocity.y;
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

        networkVelocity.Value = velocity;
        Debug.Log("movement: " + velocity.y);
    }

    void updateBodyRot()
    {
        Vector3 v = bodyDirTransform.InverseTransformDirection(networkVelocity.Value);
        v = Vector3.ClampMagnitude(v, tiltAmmount_max);
        bodyRotTransform.localRotation = Quaternion.Euler(0, v.z * tiltStrength, - v.y * tiltStrength); 
    }

    void updateWheelRot()
    {
        Vector3 v = new Vector3(networkVelocity.Value.x, 0, networkVelocity.Value.z);
        if(v != Vector3.zero)
        {
            float angle = Mathf.Atan2(v.x, v.z) * Mathf.Rad2Deg;
            wheelTransform.eulerAngles = wheelTransform.up * angle;
        }
    }

    void updateSpring()
    {
        float shift_in_units = Mathf.Clamp(networkVelocity.Value.y * springMovement_strength, -springMovement_max, springMovement_max);
        //body
        upperBodyTransform.localPosition = Vector3.right * shift_in_units;
        //spring
        Vector3 springScale = initialSpringScale;
        springScale.y *= shift_in_units * springScaling + 1;
        springTransform.localScale = springScale;
    }
}
 