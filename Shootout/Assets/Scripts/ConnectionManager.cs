using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Net;
using System.Net.Sockets;
using System;
using TMPro;
using Unity.Netcode.Transports.UTP;
using System.IO;

public class ConnectionManager : NetworkBehaviour
{
    /*
    public NetworkManager networkManager;
    public NetworkVariable<int> chosenMap;

    private void Awake()
    {
        chosenMap = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        //chosenMap.OnValueChanged += mapChoiceArrived;
    }

    //void mapChoiceArrived(int previousValue, int newValue)
    //{
    //    Manager.Instance.getMap();
    //    Spawnlocation.convertCollToBox(newValue == 1 ? Manager.Instance.oldMap : Manager.Instance.newMap);
    //}

    IEnumerator waitForChosenMap()
    {
        while(chosenMap.Value == 0) ;
            yield return null;

        Manager.Instance.getMap();
        if(chosenMap.Value == 1)
            Spawnlocation.convertCollToBox(Manager.Instance.oldMap);
        else if(chosenMap.Value == 2)
            Spawnlocation.convertCollToBox(Manager.Instance.newMap);

        while (Playermanager.ownerPlayer == null)
            yield return null;
        Playermanager.ownerPlayer.spawn();
    }

    public string connectAsHost(int pMap, ushort port = 7777)
    {
        if (networkManager.IsClient || networkManager.IsServer || networkManager.IsHost)
            return "";
        networkManager.GetComponent<UnityTransport>().ConnectionData.Address = getLocalIPAdress();
        networkManager.GetComponent<UnityTransport>().ConnectionData.Port = port;
        networkManager.StartHost();
        chosenMap.Value = pMap;
        StartCoroutine(waitForChosenMap());
        return getIPv4(getLocalIPAdress())[1]; // returns short ip
    }

    public string connectToHost(string shortIP, ushort port = 7777)
    {
        if (networkManager.IsClient || networkManager.IsServer || networkManager.IsHost)
            return "";
        networkManager.GetComponent<UnityTransport>().ConnectionData.Address = getIPv4(getLocalIPAdress())[0] + shortIP;
        networkManager.GetComponent<UnityTransport>().ConnectionData.Port = port;
        networkManager.StartClient();
        StartCoroutine(waitForConnection());
        return shortIP;
    }

    public string GlobalconnectAsHost(int pMap, ushort port = 7777)
    {
        if (networkManager.IsClient || networkManager.IsServer || networkManager.IsHost)
            return "";
        string globalIP = getGlobalIPAddress();
        if (globalIP != "")
        {
            networkManager.GetComponent<UnityTransport>().ConnectionData.Address = globalIP;
            networkManager.GetComponent<UnityTransport>().ConnectionData.Port = port;
            networkManager.StartHost();
        }
        chosenMap.Value = pMap;
        StartCoroutine(waitForConnection());
        return globalIP; // returns long ip
    }

    public string GlobalconnectToHost(string longIP6, ushort port = 7777)
    {
        if (networkManager.IsClient || networkManager.IsServer || networkManager.IsHost)
            return "";
        networkManager.GetComponent<UnityTransport>().ConnectionData.Address = longIP6;
        networkManager.GetComponent<UnityTransport>().ConnectionData.Port = port;
        networkManager.StartClient();
        StartCoroutine(waitForConnection());
        return longIP6;
    }

    IEnumerator waitForConnection()
    {
        float timer = 0;
        while (timer < 10)
        {
            timer += Time.deltaTime;
            if (networkManager.IsConnectedClient || networkManager.IsHost)
            {
                Manager.Instance.startGame();
                StartCoroutine(waitForChosenMap());
                break;
            }
            yield return null;
        }
        if (!networkManager.IsConnectedClient && !networkManager.IsHost)
        {
            stopNetwork();
            UIManager.Instance.connectionFailed();
        }
    }

    public void stopNetwork()
    {
        networkManager.Shutdown();
        StartCoroutine(resetChosenMap());
    }

    IEnumerator resetChosenMap()
    {
        while (IsClient || IsHost)
            yield return null;

        chosenMap.Value = 0;
    }

    //IEnumerator isConnected()
    //{

    //    if(!networkManager.IsConnectedClient)
    //}

    private void OnConnectedToServer()
    {
        //Debug.Log("connected");
        //Manager.Instance.startGame();
    }

    private void OnFailedToConnect()
    {
        //UIManager.Instance.connectionFailed();
    }
    
    private void OnDisconnectedFromServer()
    {
        //change
        //UIManager.Instance.connectionFailed();
    }

    public void disconnect()
    {
        //if (networkManager.IsClient)
        //    networkManager.DisconnectClient();
        //else if(networkManager.IsHost)
        //    networkManager.Shutdown();//wenn richtig
    }

    string getLocalIPAdress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }

    private string getGlobalIPAddress()
    {
        try
        {
            var url = "https://api64.ipify.org/";

            WebRequest request = WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream dataStream = response.GetResponseStream();

            using StreamReader reader = new StreamReader(dataStream);

            var ip = reader.ReadToEnd();
            reader.Close();

            return ip;
        } catch(Exception e)
        {
            //exit
            Debug.LogError(e);
            return "";
        }
    }

    string[] getIPv4(string ip) // "192.172.68.26" => { "192.172.68." , "26" }
    {
        string charedString = "";
        string lastDigits = "";
        int dotCount = 0;
        for (int i = 0; i < ip.Length; i++)
        {
            if(dotCount < 3)
            {
                if(ip[i] == '.')
                    dotCount++;
                charedString += ip[i];
            } else
                lastDigits += ip[i];
        }
        return new string[] { charedString, lastDigits};
    }

*/
}
