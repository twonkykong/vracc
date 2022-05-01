using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class badmintonRacket : MonoBehaviour
{
    public Animation anim;
    public GameObject shuttlecock, g;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            anim.Stop();
            anim.Play("hit" + UnityEngine.Random.Range(1, 3));

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 4f))
            {
                if (hit.collider.tag == "shuttlecock")
                {
                    hit.collider.transform.rotation = transform.rotation;
                    hit.collider.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 10f, ForceMode.Impulse);
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            anim.Stop();
            anim.Play("throw_shuttlecock");

            if (g != null) PhotonNetwork.Destroy(g);
            g = PhotonNetwork.Instantiate(shuttlecock.name, transform.position + transform.forward, transform.rotation);
            g.GetComponent<Rigidbody>().AddForce(Vector3.up * 3f, ForceMode.Impulse);
        }
    }
}
