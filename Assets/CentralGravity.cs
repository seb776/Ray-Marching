using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralGravity : MonoBehaviour
{
    private static float FORCE = 100.0F;
    void Update()
    {
        this.gameObject.GetComponent<Rigidbody>().AddForce(-this.transform.localPosition.normalized * FORCE * Time.deltaTime, ForceMode.Acceleration);        
    }
}
