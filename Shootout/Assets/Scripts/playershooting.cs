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
    public bool _isDead => GetComponent<Playermanager>().isDead.Value;
    Transform cam;

    PlayerInput playerInput;
    InputAction fireAction;

    public GameObject laser;

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
        if(Manager.Instance.gamestate == Manager.GameState.Game && !_isDead)
            if(IsOwner)
            {
                UIGameManager.Instance.coolDown(_timeSinceShot / coolDown);

                if (fireAction.WasPerformedThisFrame() && _timeSinceShot >= coolDown)
                {
                    _timeSinceShot = 0;
                    shoot();
                }
            }
    }

    void shoot()
    {
        RaycastHit hit;
        LayerMask layerMask = LayerMask.GetMask("Default", "Enemy", "EnemyBody");
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(cam.position, cam.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider.gameObject.layer == 8) //Layer: "Enemy"
                playerHit(hit.collider.GetComponent<playershooting>());
            if (hit.collider.gameObject.layer == 9) //Layer: "EnemyBody"
                playerHit(hit.collider.GetComponent<playerFinder>().playershootingScripct);

            laserEffectServerRPC(hit.distance);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void laserEffectServerRPC(float camDistance)
    {
        laserEffectClientRPC(camDistance);
    }

    [ClientRpc]
    public void laserEffectClientRPC(float camDistance)
    {
        Instantiate(laser).GetComponent<LaserEffect>().InitializeLaser(cam.position, cam.rotation, camDistance, playermanager.laserPointer.transform.position);
    }

    void playerHit(playershooting hitPlayer)
    {
        if (hitPlayer == this)
            return;

        //local display of a successful shot
        playermanager.shotOtherPlayer(hitPlayer.name);

        //msgToServer; global message about shot
        hitPlayer.playerShotServerRPC(name, hitPlayer.name);
    }

    [ServerRpc(RequireOwnership = false)]
    public void playerShotServerRPC(string shooter, string victim)
    {
        receiveShotClientRPC(shooter, victim);
    }

    [ClientRpc]
    public void receiveShotClientRPC(string shooter, string victim)
    {
        playermanager.gotHit(shooter, victim); //every version of this player
    }
}
