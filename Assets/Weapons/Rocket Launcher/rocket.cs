using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Photon.Pun;

public class rocket : MonoBehaviour
{
    public Collider[] col;
    public GameObject effect;
    public float force, radius;

    private void OnCollisionEnter(Collision collision)
    {
        //Instantiate(effect, transform.position, Quaternion.identity);
        GetComponent<Collider>().enabled = false;
        Vector3 pos = transform.position;
        col = Physics.OverlapSphere(transform.position, 3f);
        foreach (Collider col in col)
        {
            if (col.gameObject.GetComponent<Rigidbody>() != null)
            {
                if (col.tag == "Player") col.gameObject.GetComponent<Player>().hp -= 30;
                col.gameObject.GetComponent<Rigidbody>().AddExplosionForce(500, pos, radius, 0.3f);
                if (col.tag == "prop")
                {
                    if (col.gameObject.GetComponent<prop>() != null) col.gameObject.GetComponent<prop>().Break();
                    else if (col.gameObject.GetComponent<hunky>() != null) col.gameObject.GetComponent<hunky>().hp -= 30;
                }
            }
        }
        PhotonNetwork.Destroy(gameObject);
    }
}
