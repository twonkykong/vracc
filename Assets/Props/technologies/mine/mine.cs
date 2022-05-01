using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class mine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Rigidbody>() != null) other.GetComponent<Rigidbody>().AddForce(transform.up * 5f, ForceMode.Impulse);
        if (other.tag == "Player") other.GetComponent<Player>().hp -= 75;
        if (other.gameObject.layer == 9) other.GetComponent<hunky>().hp -= 100;

        PhotonNetwork.Destroy(transform.parent.gameObject);
    }
}
