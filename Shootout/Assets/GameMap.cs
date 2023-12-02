using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMap : MonoBehaviour
{
    [SerializeField]
    GameObject[] mapPrefab;
    static GameObject mapInstance;
    public static int currentMap = 0;

    public static GameMap Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }

    void Start() {
        loadMenuMap(false);
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
        loadMap(currentMap);
    }

    void loadMap(int mapNumber)
    {
        Destroy(mapInstance);
        mapInstance = Instantiate(mapPrefab[mapNumber]);
        currentMap = mapNumber;
        Spawnlocation.convertCollToBox(mapInstance);
    }
}
