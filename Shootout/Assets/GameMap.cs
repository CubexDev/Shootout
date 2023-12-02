using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMap : MonoBehaviour
{
    [SerializeField]
    GameObject[] mapPrefab;
    static GameObject mapInstance;
    public static int currentMap = 0;

    void Start() {
        mapInstance = Instantiate(mapPrefab[currentMap]);
    }
    private void OnEnable() {
        Manager.buildMap += loadMap;
        Manager.gameleft += loadMenuMap;
    }
    private void OnDisable() {
        Manager.buildMap -= loadMap;
        Manager.gameleft -= loadMenuMap;
    }

    void loadMenuMap(bool isConnectionError) //map for main manu
    {
        currentMap = 0;
        loadMap(0);
    }

    void loadMap(int mapNumber)
    {
        if(currentMap != mapNumber)
        {
            Destroy(mapInstance);
            mapInstance = mapPrefab[mapNumber];
            currentMap = mapNumber;
        }
        Spawnlocation.convertCollToBox(mapInstance);
    }
}
