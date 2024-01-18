using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public enum JumppadType { HighJumppad, FarJumppad };
    [SerializeField]
    JumppadType jumppadType;
    [SerializeField]
    float highVelocity;
    [SerializeField]
    Vector3 farVelocityMultiplier;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Player")
            return;

        playermovment p = other.GetComponent<playerFinder>().playershootingScripct.gameObject.GetComponent<playermovment>();

        if (jumppadType == JumppadType.HighJumppad)
            p.jumppadYVelocity(highVelocity);
        else
        {
            p.jumppadVelocityMultiplier(farVelocityMultiplier);
            p.jumppadYVelocity(highVelocity);
        }
    }
}
