using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class Playermanager : NetworkBehaviour
{
    public NetworkVariable<FixedString64Bytes> _playerName;
    public string playerNameString => _playerName.Value.ToString();
    public NetworkVariable<int> kills;
    public NetworkVariable<int> deaths;
    public NetworkVariable<bool> isDead;

    public TMP_Text nameLabel;
    public GameObject body;
    public GameObject laserPointer;
    CharacterController characterController;

    public static Playermanager ownerPlayer; //player who is owner on this device

    public delegate void Kill(Playermanager thisManager, string affectedManager);
    public Kill shotPlayer;
    public Kill gotShot;

    private void Awake()
    {
        _playerName = new NetworkVariable<FixedString64Bytes>
            ("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        kills = new NetworkVariable<int>
            (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        deaths = new NetworkVariable<int>
            (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        isDead = new NetworkVariable<bool>
            (false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        gotShot += UIGameManager.Instance.gotShot;
        shotPlayer += UIGameManager.Instance.shotPlayer;
        characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        _playerName.OnValueChanged += nameArrived;
    }

    void nameArrived(FixedString64Bytes previousName, FixedString64Bytes newName)
    {
        if(previousName.Value.ToString() == "")
        {
            if (IsOwner) _playerName.Value = PlayersManager.Instance.addPlayerOwnerSide(this, newName.Value.ToString());
            else PlayersManager.Instance.addPlayerClientSide(this);

            gameObject.name = newName.Value.ToString();
            if(!IsOwner) nameLabel.text = newName.Value.ToString();
        } else if(previousName != newName) //name changed midgame
        {
            gameObject.name = newName.Value.ToString();
            if (!IsOwner) nameLabel.text = newName.Value.ToString();
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
        {
            gameObject.layer = 8;
            body.layer = 9;
            if (playerNameString != "")
                nameArrived(new FixedString64Bytes(""), playerNameString);
        } else
        {
            ownerPlayer = this;
            _playerName.Value = new FixedString64Bytes(Manager.Instance.currentPlayerName);
            Destroy(nameLabel.gameObject);
            spawn();
        }
    }

    public override void OnNetworkDespawn()
    {
        PlayersManager.Instance.removePlayer(this);
        base.OnNetworkDespawn();
    }

    void spawn()
    {
        Vector3 newPos = Spawnlocation.getLocation();
        transform.position = newPos;
        transform.rotation = Quaternion.LookRotation(- new Vector3(0, newPos.y, 0));
    }

    public void shotOtherPlayer(string hitPlayer)
    {
        kills.Value++;
        shotPlayer?.Invoke(this, hitPlayer);
    }

    public void gotHit(string shooter, string victim)
    {
        if (IsOwner)
        {
            isDead.Value = true;
            deaths.Value++;
            gotShot?.Invoke(this, shooter);
            spawn();
            StartCoroutine(ownerPlayerDead());
        }
        else
            StartCoroutine(foreignPlayerDead());
    }

    public void respawn()
    {
        isDead.Value = false;
    }

    IEnumerator ownerPlayerDead()
    {
        Manager.Instance.stopGame();
        //_playerMovement.stopmovement
        //_playerMovement.stopphysics
        //_playerlooking.stopcameramovement
        //_playershooting.stopshoots
        //_UIGameManager.deathUI
        body.SetActive(false);
        laserPointer.SetActive(false);

        while (isDead.Value)
        {
            //zoom camera out
            yield return null;
        }

        body.SetActive(true);
        laserPointer.SetActive(true);
        //reset cam pos
        //_playerMovement.enablemovement
        //_playerMovement.enablephysics
        //_playerlooking.enablecameramovement
        //_playershooting.enableshoots
        Manager.Instance.continueGame();
        //_UIGameManager.gameUI
    }

    IEnumerator foreignPlayerDead()
    {
        //_playerMovement.stopphysics
        //_Disable all colliders and visuals
        body.SetActive(false);
        nameLabel.enabled = false;
        laserPointer.SetActive(false);
        characterController.enabled = false;

        if(!isDead.Value)
            while (!isDead.Value)
                yield return null;

        while (isDead.Value)
            yield return null;

        body.SetActive(true);
        nameLabel.enabled = true;
        laserPointer.SetActive(true);
        characterController.enabled = true;
        //_playerMovement.enablephysics
        //_enable all colliders and visuals

    }

    private void OnDisable()
    {
        if(IsOwner && Manager.Instance.gamestate != Manager.GameState.Lobby) //connection Error; nameLabel == null statt isowner
            Manager.Instance.connectionLost();
    }
}
