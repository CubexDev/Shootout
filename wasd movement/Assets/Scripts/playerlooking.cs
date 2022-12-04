using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class playerlooking : NetworkBehaviour
{
    [SerializeField] float mouseSensitivity = 3f;
    [SerializeField] Transform cameraTransform;

    PlayerInput playerInput;
    InputAction lookAction;

    Vector2 look;

    void Start()
    {
        playerInput = Manager.Instance.playerInput;
        lookAction = playerInput.actions["Look"];

        switchCameraTo(cameraTransform.gameObject);
    }

    private void Update()
    {
        if (IsOwner) //ist dieses GameObjekt das zu steuernde
            UpdateLook();
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
