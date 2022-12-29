using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class Playermanager : NetworkBehaviour
{
    //string _playerName;
    public NetworkVariable<FixedString64Bytes> _playerName;
    public string playerNameString => _playerName.Value.ToString();
    public NetworkVariable<int> kills;
    public NetworkVariable<int> deaths;

    public TMP_Text nameLabel;

    public delegate void Kill(Playermanager playermanager);
    public Kill shotPlayer;
    public Kill gotShot;

    //static Playermanager ownerPlayer;

    private void Awake()
    {
        _playerName = new NetworkVariable<FixedString64Bytes>
            ("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        kills = new NetworkVariable<int>
            (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        deaths = new NetworkVariable<int>
            (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        gotShot += UIGameManager.Instance.gotShot;
        shotPlayer += UIGameManager.Instance.shotPlayer;
    }

    private void OnEnable()
    {
        _playerName.OnValueChanged += nameArrived;
    }

    void nameArrived(FixedString64Bytes previousName, FixedString64Bytes newName)
    {
        if(previousName.Value.ToString() == "")
        {
            if(IsHost || IsServer) PlayersManager.Instance.addPlayerHostSide(this, newName.Value.ToString());
            else if(IsClient) PlayersManager.Instance.addPlayerClientSide(this);

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
            if(playerNameString != "")
                nameArrived(new FixedString64Bytes(""), playerNameString);
        } else
        {
            _playerName.Value = new FixedString64Bytes(Manager.Instance.currentPlayerName);
            Destroy(nameLabel.gameObject);
            spawn();
            //StartCoroutine(deleteThis());
        }
    }

    IEnumerator deleteThis()
    {
        yield return new WaitForSeconds(2);
        spawn();
    }

    public override void OnNetworkDespawn()
    {
        PlayersManager.Instance.removePlayer(this);
        base.OnNetworkDespawn();
    }

    private void Update()
    {
        if(IsOwner)
            updateLabels();
    }


    void updateLabels()
    {
        for (int i = 0; i < PlayersManager.Instance.players.Count; i++)
        {
            if (PlayersManager.Instance.players[i] != this)
            {
                Transform label = PlayersManager.Instance.players[i].nameLabel.transform;
                label.LookAt(transform);
            }
        }
    }

    void spawn()
    {
        Vector3 newPos = Spawnlocation.getLocation();
        transform.position = newPos;
        transform.rotation = Quaternion.LookRotation(- new Vector3(0, newPos.y, 0));
    }

    public void shotOtherPlayer()
    {
        kills.Value++;
        shotPlayer?.Invoke(this);
    }

    public void gotHit()
    {
        spawn();
        deaths.Value++;
        gotShot?.Invoke(this);
    }
}
