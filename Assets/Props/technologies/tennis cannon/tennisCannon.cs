using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class tennisCannon : MonoBehaviour
{
    public GameObject ballPrefab;
    float start;

    private void FixedUpdate()
    {
        start += 0.1f;
        if (start >= 5f)
        {
            GameObject g = PhotonNetwork.Instantiate(ballPrefab.name, transform.position + transform.right * 1.5f + transform.up / 3, Quaternion.identity);
            g.GetComponent<Rigidbody>().AddForce(transform.right * 20f + transform.up * 5f, ForceMode.Impulse);
            start = 0;
        }
    }
}
