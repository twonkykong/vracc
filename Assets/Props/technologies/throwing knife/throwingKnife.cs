using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class throwingKnife : MonoBehaviour
{
    float start;
    public GameObject knife;

    private void FixedUpdate()
    {
        start += 0.1f;
        if (start >= 5)
        {
            GameObject g = PhotonNetwork.Instantiate(knife.name, transform.position + transform.right * 1.55f + transform.up * 2.48f, transform.rotation);
            g.GetComponent<Rigidbody>().AddForce(transform.right * 15f + transform.up * 3f, ForceMode.Impulse);
            start = 0;
        }
    }
}
