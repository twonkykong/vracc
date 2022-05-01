using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class monke : MonoBehaviour
{
    public Vector3 pos;
    public GameObject head, nearestProp;
    Animator anim;

    public List<GameObject> props;
    public float speed = 1, jumpForce = 5;
    public bool walking, sit, flip, wannaHold, holding, lieDown;
    public float start, start1, start2, start3, start4, start5, start6;

    bool jumpAnimation;

    private void Start()
    {
        if (!GetComponent<PhotonView>().IsMine) return;

        pos = transform.position + new Vector3(Random.Range(-30, 30), 0, Random.Range(-30, 30));
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!GetComponent<PhotonView>().IsMine) return;

        if (walking)
        {
            GameObject[] props1 = GameObject.FindObjectsOfType<GameObject>();
            props = new List<GameObject>();
            foreach (GameObject prop in props1)
            {
                if (prop.layer == 10 && prop != gameObject) props.Add(prop);
            }

            if (wannaHold)
            {
                if (props != null)
                {
                    foreach(GameObject prop in props)
                    {
                        if (nearestProp != null)
                        {
                            if (Vector3.Distance(transform.position, prop.transform.position) < Vector3.Distance(transform.position, nearestProp.transform.position)) nearestProp = prop;
                            pos = nearestProp.transform.position;
                        }
                        else
                        {
                            nearestProp = prop;
                            pos = nearestProp.transform.position;
                        }
                    }
                }
            }

            

            sit = false;
            anim.SetBool("sit", false);
            anim.SetBool("lie down", false);
            transform.position = Vector3.MoveTowards(transform.position, pos, speed);
            transform.LookAt(new Vector3(pos.x, transform.position.y, pos.z));
            pos.y = transform.position.y;
            head.transform.LookAt(pos);
            if (transform.position == pos) pos = transform.position + new Vector3(Random.Range(-30, 30), 0, Random.Range(-30, 30));
            anim.SetBool("walk", true);

            Debug.DrawRay(transform.position, transform.forward, Color.yellow);
            Debug.DrawRay(transform.position + transform.up * 0.7f, transform.forward, Color.green);
            Debug.DrawRay(transform.position, -transform.up, Color.red);
            if (Physics.Raycast(transform.position, transform.forward, 1f))
            {
                if (Physics.Raycast(transform.position + transform.up * 0.7f, transform.forward, 1f))
                {
                    if (Physics.Raycast(transform.position, -transform.up, 2f)) pos = transform.position + new Vector3(Random.Range(-30, 30), 0, Random.Range(-30, 30));
                }
                else
                {
                    if (jumpAnimation == false) GetComponent<Rigidbody>().AddForce(transform.up * jumpForce, ForceMode.Impulse);
                    jumpAnimation = true;
                }
            }
        }
        else
        {
            anim.SetBool("walk", false);
            if (sit == false && lieDown == false)
            {
                anim.SetBool("idle", true);
                anim.SetBool("sit", false);
                anim.SetBool("lie down", false);
            }
            else if (lieDown == true && sit == false)
            {
                anim.SetBool("idle", false);
                anim.SetBool("sit", false);
                anim.SetBool("lie down", true);
            }
            else if (sit == true && lieDown == false)
            {
                anim.SetBool("sit", true);
                anim.SetBool("idle", false);
                anim.SetBool("lie down", false);
            }
        }

        if (holding)
        {
            nearestProp.transform.position = head.transform.position + transform.up;
        }
    }

    private void FixedUpdate()
    {
        if (!GetComponent<PhotonView>().IsMine) return;

        if (jumpAnimation)
        {
            anim.SetBool("jump", true);
            start += 0.1f;
            if (start >= 1)
            {
                anim.SetBool("jump", false);
                jumpAnimation = false;
                start = 0;
            }
        }

        if (sit == true)
        {
            start1 += 0.1f;
            if (start1 >= Random.Range(20, 100))
            {
                sit = false;
                walking = true;
                start1 = 0;
            }
        }

        if (walking == false && sit == false)
        {
            start3 += 0.1f;
            if (start3 >= Random.Range(10, 100))
            {
                sit = false;
                walking = true;
                start3 = 0;
            }
        }

        if (walking == true)
        {
            start2 += 0.1f;
            if (start2 >= Random.Range(50, 150))
            {
                walking = false;
                int rand = Random.Range(0, 3);
                if (rand == 1) sit = true;
                else if (rand == 2) lieDown = true;
                else
                {
                    if (Random.Range(0, 3) == 1) flip = true;
                    else if (Random.Range(0, 3) == 1) wannaHold = true;
                }
                start2 = 0;
            }
        }

        if (flip == true)
        {
            anim.SetBool("flip", true);
            start4 += 0.1f;
            if (start4 >= 5)
            {
                anim.SetBool("flip", false);
                start4 = 0;
                flip = false;
                int rand = Random.Range(0, 3);
                if (rand == 1) sit = true;
                else if (rand == 2) lieDown = true;
            }
        }

        if (holding == true)
        {
            start5 += 0.1f;
            if (start5 >= Random.Range(50, 150))
            {
                start5 = 0;
                holding = false;
                nearestProp = null;
            }
        }

        if (lieDown == true)
        {
            start6 += 0.1f;
            if (start6 >= Random.Range(50, 100))
            {
                start6 = 0;
                lieDown = false;
                walking = true;
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (wannaHold)
        {
            if (collision.collider.gameObject == nearestProp)
            {
                if (nearestProp.transform.parent != null) nearestProp = nearestProp.transform.parent.gameObject;
                wannaHold = false;
                holding = true;
            }
        }
    }
}
