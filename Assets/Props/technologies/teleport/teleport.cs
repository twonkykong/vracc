using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleport : MonoBehaviour
{
    public GameObject teleportTo;
    private void Update()
    {
        if (teleportTo == null) PhotonNetwork.Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Rigidbody>() != null)
        {
            other.transform.position = teleportTo.transform.position + transform.up * 2;
            other.GetComponent<Rigidbody>().AddForce(transform.up * 7, ForceMode.Impulse);
        }
    }
}
