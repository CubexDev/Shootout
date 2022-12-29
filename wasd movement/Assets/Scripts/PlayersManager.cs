using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class PlayersManager : NetworkBehaviour
{
    public static PlayersManager Instance;

    public List<Playermanager> players = new List<Playermanager>();
    //NetworkList<FixedString64Bytes> playerNames;
    //NetworkList<int> playerKills;
    //NetworkList<int> playerDeaths;

    //public List<GameObject> playerObjects { get; private set; } = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);

        //playerNames = new NetworkList<FixedString64Bytes>
        //(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        //playerKills = new NetworkList<int>();
        //playerDeaths = new NetworkList<int>();
    }

    public void addPlayerClientSide(Playermanager player)
    {
        players.Add(player);
        Debug.Log(player.playerNameString + " joined the Game");
    }
    public string addPlayerHostSide(Playermanager player, string name)
    {
        string newName = name;
        if (IsHost || IsServer)
            newName = getUniqueName(name);
        players.Add(player);
        Debug.Log(player.playerNameString + "Joined the Game");
        return newName;
    }

    public void removePlayer(Playermanager player)
    {
        players.Remove(player);
        Debug.Log(player.playerNameString + " left the Game");
    }

    string getUniqueName(string name)
    {
        Playermanager findName = getPlayerByName(name);
        int counter = 0;
        while (findName != null)
        {
            findName = getPlayerByName(name + " #" + counter);
            counter++;
        }
        string newName = name;
        if (counter != 0)
            newName = name + " #" + counter;
        return newName;
    }

    Playermanager getPlayerByName(string name)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].playerNameString == name)
                return players[i];
        }
        return null;
    }
}
