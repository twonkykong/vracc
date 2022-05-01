using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class baseballBat : MonoBehaviour
{
    public Animation anim;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            anim.Stop();
            anim.Play("hit" + Random.Range(1, 3));
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 4f))
            {
                if (hit.collider.gameObject.GetComponent<Rigidbody>() != null)
                {
                    hit.collider.gameObject.GetComponent<Rigidbody>().AddForce(-(transform.position - hit.collider.transform.position) * 5f, ForceMode.Impulse);
                    if (hit.collider.tag == "Player") hit.collider.gameObject.GetComponent<Player>().hp -= 10;
                    else if (hit.collider.tag == "prop")
                    {
                        if (hit.collider.gameObject.GetComponent<prop>() != null) hit.collider.gameObject.GetComponent<prop>().Break();
                        else if (hit.collider.gameObject.GetComponent<hunky>() != null) hit.collider.gameObject.GetComponent<hunky>().hp -= 25;
                    }
                }
            }
        }
    }
}
