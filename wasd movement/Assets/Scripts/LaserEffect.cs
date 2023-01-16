using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEffect : MonoBehaviour
{
    public float effectTime = 0.3f;
    public int effectWidth = 40;

    private void Start()
    {
        StartCoroutine(destroySelf());
    }

    public void InitializeLaser(Vector3 camPos, Quaternion camRotation, float camDistance, Vector3 laserPointerPos)
    {
        Vector3 laserStart = laserPointerPos;
        Vector3 laserEnd = (camRotation * Vector3.forward).normalized * camDistance + camPos;
        Quaternion laserRot = Quaternion.LookRotation(laserStart - laserEnd, Vector3.up);
        float laserDistance = Vector3.Distance(laserStart, laserEnd);

        transform.position = laserStart;
        transform.rotation = laserRot;
        transform.localScale = new Vector3(effectWidth, effectWidth, 100 * laserDistance);
    }

    IEnumerator destroySelf()
    {
        yield return new WaitForSeconds(effectTime);
        Destroy(gameObject);
    }
}
