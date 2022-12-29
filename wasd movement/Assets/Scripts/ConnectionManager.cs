using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using TMPro;
using Unity.Netcode.Transports.UTP;

public class ConnectionManager : MonoBehaviour
{
    public NetworkManager networkManager;

    private void Awake()
    {
        Debug.Log(getLocalIPAdress(6));
    }

    public string connectAsHost(ushort port = 8009)
    {
        if (networkManager.IsClient || networkManager.IsServer || networkManager.IsHost)
            return "";
        networkManager.GetComponent<UnityTransport>().ConnectionData.Address = getLocalIPAdress(4);
        networkManager.GetComponent<UnityTransport>().ConnectionData.Port = port;
        networkManager.StartHost();
        return getIPv4(getLocalIPAdress(4))[1]; // returns short ip
    }

    public void connectToHost(string shortIP, ushort port = 8009)
    {
        if (networkManager.IsClient || networkManager.IsServer || networkManager.IsHost)
            return;
        networkManager.GetComponent<UnityTransport>().ConnectionData.Address = getIPv4(getLocalIPAdress(4))[0] + shortIP;
        networkManager.GetComponent<UnityTransport>().ConnectionData.Port = port;
        networkManager.StartClient();
    }

    public string GlobalconnectAsHost(ushort port = 8009)
    {
        if (networkManager.IsClient || networkManager.IsServer || networkManager.IsHost)
            return "";
        networkManager.GetComponent<UnityTransport>().ConnectionData.Address = getLocalIPAdress(6);
        networkManager.GetComponent<UnityTransport>().ConnectionData.Port = port;
        networkManager.StartHost();
        return getLocalIPAdress(6); // returns long ip
    }

    public void GlobalconnectToHost(string longIP6, ushort port = 8009)
    {
        if (networkManager.IsClient || networkManager.IsServer || networkManager.IsHost)
            return;
        networkManager.GetComponent<UnityTransport>().ConnectionData.Address = getIPv4(getLocalIPAdress(4))[0] + longIP6;
        networkManager.GetComponent<UnityTransport>().ConnectionData.Port = port;
        networkManager.StartClient();
    }

    public void disconnect()
    {
        //if (networkManager.IsClient)
        //    networkManager.DisconnectClient();
        //else if(networkManager.IsHost)
        //    networkManager.Shutdown();//wenn richtig
    }

    string getLocalIPAdress(int addressFamily)
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == (addressFamily == 4 ? AddressFamily.InterNetwork : AddressFamily.InterNetworkV6))
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
