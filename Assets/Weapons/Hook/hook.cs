using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class hook : MonoBehaviour
{
    bool hooking;
    public GameObject g, prefab, player;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 10f)) 
            {
                g = Instantiate(prefab, hit.point, Quaternion.identity);
                hooking = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            hooking = false;
            player.GetComponent<SpringJoint>().connectedBody = null;
            Destroy(g);
        }

        if (hooking == true)
        {
            player.GetComponent<SpringJoint>().connectedBody = g.GetComponent<Rigidbody>();
        }
    }
}
