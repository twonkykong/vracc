using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jumpPad : MonoBehaviour
{
    public float force = 10f;
    public GameObject col;

    private void OnTriggerEnter (Collider other)
    {
        col = other.gameObject;
        if (col.GetComponent<Rigidbody>() != null)
        {
            col.GetComponent<Rigidbody>().AddForce(transform.up * 10 / col.GetComponent<Rigidbody>().mass, ForceMode.Impulse);
        }
    }
}
