using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Networking;

public class ConnectionManager : MonoBehaviour
{
    public NetworkManager networkManager;

    public void ConnectAsHost()
    {
        if (networkManager.IsClient || networkManager.IsServer || networkManager.IsHost)
            return;
        networkManager.StartHost();
        
        networkManager.StartClient();
        
    }
}
