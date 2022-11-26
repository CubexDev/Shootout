using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class playermovment : NetworkBehaviour
{
    [SerializeField] float mouseSensitivity = 3f;
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float mass = 1f;
    [SerializeField] Transform cameraTransform;
    
    CharacterController controller;
    Vector3 velocity;
    Vector2 look;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        UpdateGravity();
        UpdateMovement();
        UpdateLook();
    }

    void UpdateGravity()
    {
        var gravity = Physics.gravity * mass * Time.deltaTime;
        velocity.y = controller.isGrounded ? -1f : velocity.y + gravity.y;
    }
    
    void UpdateMovement()
    {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        
        var input = new Vector3();
        input += transform.forward * y;
        input += transform.right * x;
        input = Vector3.ClampMagnitude(input, 1f); // diagnol movement = forward movement

        if (Input.GetButtonDown("Jump") && controller.isGrounded);
        {
            velocity.y += jumpSpeed;
        }

        controller.Move((input * movementSpeed + velocity) * Time.deltaTime);
    }

    void UpdateLook()
    {
        look.x += Input.GetAxis("Mouse X");
        look.y += Input.GetAxis("Mouse Y");

        look.y = Mathf.Clamp(look.y, -89f, 89f);

        cameraTransform.localRotation = Quaternion.Euler(-look.y, 0, 0);
        transform.localRotation = Quaternion.Euler(0, look.x, 0);
    }
}
