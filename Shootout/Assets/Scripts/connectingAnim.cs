using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class connectingAnim : MonoBehaviour
{
    public TMP_Text connectingTxt;
    float speed = 0.3f;
    float timer = 0;
    float overallTimer = 0;

    private void OnEnable()
    {
        timer = 0;
        overallTimer = 0;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        overallTimer += Time.deltaTime;
        if (timer < speed)
            connectingTxt.text = "connecting";
        else if (timer < speed * 2)
            connectingTxt.text = "connecting.";
        else if (timer < speed * 3)
            connectingTxt.text = "connecting..";
        else if (timer < speed * 4)
            connectingTxt.text = "connecting...";
        else timer = 0;
        if (overallTimer > 10)
            UIManager.Instance.connectionFailed();
    }
}
