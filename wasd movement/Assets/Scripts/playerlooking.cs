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
    public float zoomOutSpeed = 1f;
    public float zoomOutDistance = 1f;

    public bool _isDead => Playermanager.ownerPlayer.isDead.Value;

    PlayerInput playerInput;
    InputAction lookAction;

    Vector2 look;

    void Start()
    {
        playerInput = Manager.Instance.playerInput;
        lookAction = playerInput.actions["Look"];
        if (IsOwner)
        {
            switchCameraTo(cameraTransform.gameObject);
            Playermanager.ownerPlayer.gotShot += cameraStartZoom;
        }
    }

    private void Update()
    {
        if (IsOwner && !_isDead) //ist dieses GameObjekt das zu steuernde
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

    void cameraStartZoom(Playermanager pm, string otherPlayer)
    {
        StartCoroutine(cameraZoomAnim(pm, otherPlayer));
    }

    IEnumerator cameraZoomAnim(Playermanager pm, string otherPlayer)
    {
        Vector3 cameraGlobalPos = cameraTransform.position;
        Quaternion cameraGlobalRot = cameraTransform.rotation;
        //rotate camera
        float timeSinceDeath = 0f;
        while (_isDead)
        {
            timeSinceDeath += Time.deltaTime;
            float fnct = (-Mathf.Exp(-timeSinceDeath * zoomOutSpeed) + 1) * zoomOutDistance;
            cameraTransform.position = cameraGlobalPos - fnct * cameraTransform.forward;
            cameraTransform.rotation = cameraGlobalRot;

            yield return null;
        }
        cameraTransform.position = transform.position + new Vector3(0, 0.88f, 0);
        lookTowardsCenter();
    }

    void lookTowardsCenter()
    {
        look.x = transform.localRotation.eulerAngles.y;
        look.y = 0;
    }

    void updateLook()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        look.x += lookInput.x * SettingsManager.c_Sensi;
        look.y += lookInput.y * SettingsManager.c_Sensi;

        look.y = Mathf.Clamp(look.y, -89f, 89f);

        transform.localRotation = Quaternion.Euler(0, look.x, 0);
        cameraTransform.localRotation = Quaternion.Euler(-look.y, 0, 0);
    }

    void switchCameraTo(GameObject go)
    {
        //for (int i = 0; i < Camera.allCamerasCount; i++)
        //{
        //    if (Camera.allCameras[i].gameObject != go) Destroy(Camera.allCameras[i].gameObject);
        //}
        go.GetComponent<Camera>().enabled = true;
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
}
