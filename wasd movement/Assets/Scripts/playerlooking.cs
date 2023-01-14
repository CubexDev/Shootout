using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using TMPro;

public class playerlooking : NetworkBehaviour
{
    [SerializeField] float mouseSensitivity = 3f;
    public Transform cameraTransform;

    PlayerInput playerInput;
    InputAction lookAction;

    Vector2 look;


    void Start()
    {
        playerInput = Manager.Instance.playerInput;
        lookAction = playerInput.actions["Look"];
        if (IsOwner)
            switchCameraTo(cameraTransform.gameObject);
    }

    private void Update()
    {
        if (IsOwner) //ist dieses GameObjekt das zu steuernde
        {
            updateLook();
        }
    }

    private void LateUpdate()
    {
        // LATE UPDATE!!
        if (IsOwner)
            updateLabels();
    }


    void updateLabels()
    {
        for (int i = 0; i < PlayersManager.Instance.players.Count; i++)
        {
            if (PlayersManager.Instance.players[i].gameObject != gameObject)
            {
                Transform label = PlayersManager.Instance.players[i].nameLabel.transform;
                label.LookAt(cameraTransform);
            }
        }
    }

    void updateLook()
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
        //for (int i = 0; i < Camera.allCamerasCount; i++)
        //{
        //    if (Camera.allCameras[i].gameObject != go) Destroy(Camera.allCameras[i].gameObject);
        //}
        go.GetComponent<Camera>().enabled = true;
    }
}
