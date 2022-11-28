using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Net;
using System.Net.Sockets;
using TMPro;
using Unity.Netcode.Transports.UTP;

public class ConnectionManager : MonoBehaviour
{
    public NetworkManager networkManager;
    /*public GameObject connectionTypePanel;
    public GameObject connectAsClientPanel;
    public GameObject connectedAsHostPanel;
    public TMP_Text lobbycodeText;
    public TMP_InputField lobbycodeField;*/


    //public void connectAsHost()
    //{
    //    if (networkManager.IsClient || networkManager.IsServer || networkManager.IsHost)
    //        return;
    //    networkManager.GetComponent<UnityTransport>().ConnectionData.Address = getLocalIPAdress();
    //    networkManager.StartHost();
    //    connectionTypePanel.SetActive(false);
    //    connectedAsHostPanel.SetActive(true);
    //    lobbycodeText.text = "Your Lobbycode is: " + getIPv4(getLocalIPAdress())[1];
    //}

    //public void startConnectionAsClient()
    //{
    //    if (networkManager.IsClient || networkManager.IsServer || networkManager.IsHost)
    //        return;
    //    connectionTypePanel.SetActive(false);
    //    connectAsClientPanel.SetActive(true);
    //}

    //public void connectToHost()
    //{
    //    if (networkManager.IsClient || networkManager.IsServer || networkManager.IsHost)
    //        return;
    //    networkManager.GetComponent<UnityTransport>().ConnectionData.Address = getIPv4(getLocalIPAdress())[0] + lobbycodeField.text;
    //    Debug.Log(getIPv4(getLocalIPAdress())[0] + lobbycodeField.text);
    //    networkManager.StartClient();
    //    connectAsClientPanel.SetActive(false);
    //}

    public string connectAsHost(ushort port)
    {
        if (networkManager.IsClient || networkManager.IsServer || networkManager.IsHost)
            return "";
        networkManager.GetComponent<UnityTransport>().ConnectionData.Address = getLocalIPAdress();
        networkManager.GetComponent<UnityTransport>().ConnectionData.Port = port;
        networkManager.StartHost();
        return getIPv4(getLocalIPAdress())[1]; // returns short ip
    }

    public string connectAsHost()
    {
        if (networkManager.IsClient || networkManager.IsServer || networkManager.IsHost)
            return "";
        networkManager.GetComponent<UnityTransport>().ConnectionData.Address = getLocalIPAdress();
        networkManager.StartHost();
        return getIPv4(getLocalIPAdress())[1]; // returns short ip
    }

    public void connectToHost(string shortIP, ushort port)
    {
        if (networkManager.IsClient || networkManager.IsServer || networkManager.IsHost)
            return;
        networkManager.GetComponent<UnityTransport>().ConnectionData.Address = getIPv4(getLocalIPAdress())[0] + shortIP;
        networkManager.GetComponent<UnityTransport>().ConnectionData.Port = port;
        networkManager.StartClient();
    }

    public void connectToHost(string shortIP)
    {
        if (networkManager.IsClient || networkManager.IsServer || networkManager.IsHost)
            return;
        networkManager.GetComponent<UnityTransport>().ConnectionData.Address = getIPv4(getLocalIPAdress())[0] + shortIP;
        networkManager.StartClient();
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
