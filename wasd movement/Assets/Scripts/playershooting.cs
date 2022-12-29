using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class playershooting : NetworkBehaviour
{
    [SerializeField] float coolDown;
    float _timeSinceShot;

    Playermanager playermanager; // playermaner of this gameobject
    Transform cam;

    PlayerInput playerInput;
    InputAction fireAction;

    private void Start()
    {
        playerInput = Manager.Instance.playerInput;
        fireAction = playerInput.actions["Fire"];
        playermanager = GetComponent<Playermanager>();
        cam = GetComponent<playerlooking>().cameraTransform;
        _timeSinceShot = coolDown;
    }

    private void Update()
    {
        checkForShoot();
        _timeSinceShot += Time.deltaTime;
    }

    void checkForShoot()
    {
        if(Manager.Instance.gamestate == Manager.GameState.Game)
            if(fireAction.WasPerformedThisFrame() && IsOwner)
                if(_timeSinceShot >= coolDown)
                {
                    _timeSinceShot = 0;
                    shoot();
                }
    }

    void shoot()
    {
        RaycastHit hit;
        LayerMask layerMask = LayerMask.GetMask("Enemy");
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(cam.position, cam.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            playerHit(hit.collider.GetComponent<playershooting>());

            Debug.DrawRay(cam.position, cam.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit " + hit.collider.gameObject.name);
        }
    }

    void playerHit(playershooting hitPlayer)
    {
        if (hitPlayer == playermanager)
            return;

        //local display of a successful shot
        displayHit();

        //msgToServer; global message about shot
        hitPlayer.playerShotServerRPC(OwnerClientId, hitPlayer.OwnerClientId);
    }

    void displayHit()
    {
        playermanager.shotOtherPlayer();
    }

    [ServerRpc(RequireOwnership = false)]
    public void playerShotServerRPC(ulong shooter, ulong victim)
    {
        receiveShotClientRPC(shooter, victim);
    }

    [ClientRpc]
    public void receiveShotClientRPC(ulong shooter, ulong victim)
    {
        if (IsOwner)
        {
            Debug.Log("received Shot: ");
            playermanager.gotHit();
        }
    }
}
