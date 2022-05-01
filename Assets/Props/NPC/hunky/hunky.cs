using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class hunky : MonoBehaviour
{
    float start, start1, end, end1;
    public bool walk, holding, talking;
    public Vector3 pos, pos1;
    public float hp = 100, distance = 5f, speed;
    GameObject holdObj;
    public GameObject head, ragdoll;
    public Animation anim;
    bool animated;

    public GameObject[] players, props;

    private void Start()
    {
        pos = transform.position + new Vector3(Random.Range(-30, 30), 0, Random.Range(-30, 30));
    }

    private void Update()
    {
        end = Random.Range(20, 200);
        end1 = Random.Range(20, 200);

        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject prop in GameObject.FindObjectsOfType<GameObject>())
        {
            if (prop.layer == 10) props.Append(prop);
        }

        transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);

        foreach (GameObject player in players)
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= distance)
            {
                walk = false;
                transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
                head.transform.LookAt(player.GetComponentInChildren<Camera>().gameObject.transform.position);
                if (animated == false)
                {
                    anim.Stop();
                    anim.Play("hello hunky");
                    animated = true;
                }
                
            }
            else
            {
                head.transform.rotation = transform.rotation;
                walk = true;
                animated = false;
            }
        }

        if (walk)
        {
            transform.position = Vector3.MoveTowards(transform.position, pos, speed / 10);
            transform.LookAt(pos);

            if (transform.position == pos)
            {
                pos = transform.position + new Vector3(Random.Range(-30, 30), 0, Random.Range(-30, 30));
            }

            if (holding)
            {
                holdObj.transform.position = transform.position + transform.up * 2f;
                holdObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
                holdObj.GetComponent<Rigidbody>().useGravity = false;
            }
            else
            {
                holdObj = null;
            }

            if (!Physics.Raycast(transform.position + transform.forward * 2, - transform.up))
            {
                pos = transform.position + new Vector3(Random.Range(-30, 30), 0, Random.Range(-30, 30));
                start = 0f;
            }
        }

        if (hp <= 0)
        {
            GameObject g = PhotonNetwork.Instantiate(ragdoll.name, transform.position, transform.rotation);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (walk)
        {
            start += 0.1f;
            if (start >= end)
            {
                pos = transform.position + new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
                start = 0f;
            }
        }

        if (holding)
        {
            start1 += 0.1f;
            if (start1 >= end1)
            {
                walk = false;
                holdObj.GetComponent<Rigidbody>().useGravity = true;
                holdObj.GetComponent<Rigidbody>().AddForce(transform.up * 2f - transform.forward * 5f, ForceMode.Impulse);
                holding = false;
                pos = transform.position + new Vector3(Random.Range(-30, 30), 0, Random.Range(-30, 30));
                walk = true;
                start1 = 0f;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (holding == false)
        {
            GameObject col = collision.collider.gameObject;

            if (col.GetComponent<Rigidbody>() != null && col.GetComponent<Rigidbody>().isKinematic == false)
            {
                if (col.layer == 10 && col.GetComponent<hunky>() == null)
                {
                    holdObj = col;
                    holding = true;
                }
            }
        }
        else
        {
            pos = transform.position + new Vector3(Random.Range(-30, 30), 0, Random.Range(-30, 30));
        }
    }
}
