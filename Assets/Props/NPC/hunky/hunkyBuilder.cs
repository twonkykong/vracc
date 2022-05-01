using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class hunkyBuilder : MonoBehaviour
{
    public Vector3 pos;
    public bool building, grounded;
    public GameObject prefab;
    public float speed = 1;

    private void Start()
    {
        pos = transform.position + new Vector3(Random.Range(-30, 30), 0, Random.Range(-30, 30));
    }
    private void Update()
    {
        if (!building)
        {
            pos = new Vector3(pos.x, transform.position.y, pos.z);
            transform.position = Vector3.MoveTowards(transform.position, pos, speed);
            transform.LookAt(pos);

            if (transform.position == pos) building = true;
        }
        else
        {
            if (grounded)
            {
                GetComponent<Rigidbody>().AddForce(transform.up * 7f, ForceMode.Impulse);
                Instantiate(prefab, transform.position - transform.forward, Quaternion.identity);
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        grounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        grounded = false;
    }
}
