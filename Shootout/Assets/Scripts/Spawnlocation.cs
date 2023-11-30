using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnlocation : MonoBehaviour
{
    public class SpawnBox
    {
        Vector3 corner1;
        Vector3 corner2;

        public SpawnBox(Vector3 corner1, Vector3 corner2)
        {
            this.corner1 = corner1;
            this.corner2 = corner2;
        }

        public Vector3 getSpawnPos()
        {
            Vector3 location;
            location.x = Random.Range(corner1.x, corner2.x);
            location.y = Random.Range(corner1.y, corner2.y);
            location.z = Random.Range(corner1.z, corner2.z);
            return location;
        }
    }

    public static Spawnlocation Instance;
    public List<SpawnBox> spawnBoxes = new List<SpawnBox>();
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }

    public static void convertCollToBox(GameObject currentMap)
    {
        Instance.spawnBoxes.Clear();
        BoxCollider[] spawnColl = currentMap.GetComponents<BoxCollider>();
        for (int i = 0; i < spawnColl.Length; i++)
        {
            Instance.spawnBoxes.Add(new SpawnBox(spawnColl[i].bounds.min, spawnColl[i].bounds.max));
        }
    }

    public static Vector3 getLocation()
    {
        return Instance.spawnBoxes[Random.Range(0, Instance.spawnBoxes.Count)].getSpawnPos();
    }
}
