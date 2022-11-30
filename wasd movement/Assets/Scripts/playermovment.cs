using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System;

public class playermovment : NetworkBehaviour
{
    [SerializeField] float mouseSensitivity = 3f;
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float mass = 1f;
    [SerializeField] float acceleration = 20f;
    [SerializeField] Transform cameraTransform;
    
    public bool isGrounded => controller.isGrounded;

    public event Action OnBeforeMove;
    public event Action<bool> OnGroundStateChange;

    internal float movementSpeedMulitplier;
    
    
    CharacterController controller;
    internal Vector3 velocity;
    Vector2 look;
    private bool  wasGrounded;

    PlayerInput playerInput;
    InputAction moveAction;
    InputAction lookAction;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["move"];
        lookAction = playerInput.actions["look"];
    }
    void Start()
    {
        switchCameraTo(cameraTransform.gameObject);
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        UpdateGround();
        UpdateGravity();
        UpdateMovement();
        UpdateLook();
    }

    void UpdateGround()
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
        velocity.y = controller.isGrounded ? -1f : velocity.y + gravity.y;
    }
    
    Vector3 GetMovementInput()
    {
        var moveInput = moveAction.ReadValue<Vector2>();
        Vector3 input = new Vector3();
        input += transform.forward * moveInput.y;
        input += transform.right * moveInput.x;
        input = Vector3.ClampMagnitude(input, 1f); // diagnol movement = forward movement
        input *= movementSpeed;
        return input;
    }
    
    
    
    void UpdateMovement()
    {
        var input = GetMovementInput();

        var factor = acceleration * Time.deltaTime;
        velocity.x = Mathf.Lerp(velocity.x, input.x, factor);
        velocity.z = Mathf.Lerp(velocity.z, input.z, factor);

        controller.Move(velocity * Time.deltaTime);
    }

    void UpdateLook()
    {
        var lookInput = lookAction.ReadValue<Vector2>();
        look.x += lookInput.x * mouseSensitivity;
        look.y += lookInput.y * mouseSensitivity;

        look.y = Mathf.Clamp(look.y, -89f, 89f);

        cameraTransform.localRotation = Quaternion.Euler(-look.y, 0, 0);
        transform.localRotation = Quaternion.Euler(0, look.x, 0);
    }

    void switchCameraTo(GameObject go)
    {
        for (int i = 0; i < Camera.allCamerasCount; i++)
        {
            if (Camera.allCameras[i].gameObject != go) Destroy(Camera.allCameras[i].gameObject);
        }
        Cursor.lockState = CursorLockMode.Locked;
    }
}
