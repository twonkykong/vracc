using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class rocketLauncher : MonoBehaviour
{
    public GameObject rocket;
    public Animation anim;
    bool canShot = true;
    public float start, endTimer = 5f;
    public Vector3 pos;
    public Quaternion rot;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) shoot();
    }

    private void FixedUpdate()
    {
        if (!canShot)
        {
            anim.Play("reload");
            start += 0.1f;
            if (start >= endTimer)
            {
                canShot = true;
                start = 0f;
            }
        }
    }

    public void shoot()
    {
        if (canShot)
        {
            GameObject g = PhotonNetwork.Instantiate(rocket.name, pos, transform.rotation);
            g.GetComponent<Rigidbody>().AddForce(transform.forward * 20f, ForceMode.Impulse);
            canShot = false;
        }
    }
}