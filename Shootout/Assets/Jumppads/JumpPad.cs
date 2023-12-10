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
        if (jumppadType == JumppadType.HighJumppad)
            other.GetComponentInParent<playermovment>().jumppadYVelocity(highVelocity);
        else
        {
            other.GetComponentInParent<playermovment>().jumppadVelocityMultiplier(farVelocityMultiplier);
            other.GetComponentInParent<playermovment>().jumppadYVelocity(highVelocity);
        }
    }
}
