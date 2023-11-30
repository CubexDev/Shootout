using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class testscript : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(test());
    }
    IEnumerator test()
    {
        yield return new WaitForSeconds(1);
        if(!Application.isEditor)
            NetworkManager.Singleton.StartHost();
    }
}
