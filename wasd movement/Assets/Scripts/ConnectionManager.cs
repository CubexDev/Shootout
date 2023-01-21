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
    public NetworkManager networkManager;

    public string connectAsHost(ushort port = 7777)
    {
        if (networkManager.IsClient || networkManager.IsServer || networkManager.IsHost)
            return "";
        networkManager.GetComponent<UnityTransport>().ConnectionData.Address = getLocalIPAdress();
        networkManager.GetComponent<UnityTransport>().ConnectionData.Port = port;
        networkManager.StartHost();
        return getIPv4(getLocalIPAdress())[1]; // returns short ip
    }

    public void connectToHost(string shortIP, ushort port = 7777)
    {
        if (networkManager.IsClient || networkManager.IsServer || networkManager.IsHost)
            return;
        networkManager.GetComponent<UnityTransport>().ConnectionData.Address = getIPv4(getLocalIPAdress())[0] + shortIP;
        networkManager.GetComponent<UnityTransport>().ConnectionData.Port = port;
        networkManager.StartClient();
        StartCoroutine(waitForConnection());
    }

    public string GlobalconnectAsHost(ushort port = 7777)
    {
        if (networkManager.IsClient || networkManager.IsServer || networkManager.IsHost)
            return "";
        networkManager.GetComponent<UnityTransport>().ConnectionData.Address = getGlobalIPAddress();
        networkManager.GetComponent<UnityTransport>().ConnectionData.Port = port;
        networkManager.StartHost();
        return getGlobalIPAddress(); // returns long ip
    }

    public void GlobalconnectToHost(string longIP6, ushort port = 7777)
    {
        if (networkManager.IsClient || networkManager.IsServer || networkManager.IsHost)
            return;
        networkManager.GetComponent<UnityTransport>().ConnectionData.Address = longIP6;
        networkManager.GetComponent<UnityTransport>().ConnectionData.Port = port;
        networkManager.StartClient();
        StartCoroutine(waitForConnection());
    }

    IEnumerator waitForConnection()
    {
        float timer = 0;
        while (timer < 5)
        {
            timer += Time.deltaTime;
            if (networkManager.IsConnectedClient)
            {
                Manager.Instance.startGame();
                break;
            }
            yield return null;
        }
        if (!networkManager.IsConnectedClient)
        {
            stopNetwork();
            UIManager.Instance.connectionFailed();
        }
    }

    public void stopNetwork()
    {
        networkManager.Shutdown();
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

}
