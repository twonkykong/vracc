using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class knife : MonoBehaviour
{
    public GameObject throwingKnife;
    public Animation anim;
    bool canThrow = true;
    public float start, endTimer = 1f;
    public Vector3 pos;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) hit();
        if (Input.GetMouseButtonDown(1)) throwKnife();
    }

    private void FixedUpdate()
    {
        if (!canThrow)
        {
            start += 0.1f;
            if (start >= endTimer)
            {
                canThrow = true;
                start = 0f;
            }
        }
    }

    public void hit()
    {
        anim.Stop();
        anim.Play("hit");
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 4f))
        {
            if (hit.collider.gameObject.GetComponent<Rigidbody>() != null)
            {
                hit.collider.gameObject.GetComponent<Rigidbody>().AddForce(-(transform.position - hit.collider.transform.position) * 2f, ForceMode.Impulse);
                if (hit.collider.tag == "Player") hit.collider.gameObject.GetComponent<Player>().hp -= 25;
                else if (hit.collider.tag == "prop")
                {
                    if (hit.collider.gameObject.GetComponent<prop>() != null) hit.collider.gameObject.GetComponent<prop>().Break();
                    else if (hit.collider.gameObject.GetComponent<hunky>() != null) hit.collider.gameObject.GetComponent<hunky>().hp -= 25;
                }
            }
        }
    }

    public void throwKnife()
    {
        if (canThrow)
        {
            anim.Play("hit");
            GameObject g = PhotonNetwork.Instantiate(throwingKnife.name, pos, transform.rotation);
            g.GetComponent<Rigidbody>().AddForce(transform.forward * 20f, ForceMode.Impulse);
            canThrow = false;
        }
    }
}
