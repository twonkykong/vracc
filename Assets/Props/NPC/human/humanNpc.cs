using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class humanNpc : MonoBehaviour
{
    public bool building, grounded, timer;
    public GameObject cubePrefab;
    public Animator anim;
    public Vector3 pos;

    float start, end = 5;
    int count;

    private void Start()
    {
        pos = transform.position + new Vector3(Random.Range(-30, 30), 0, Random.Range(-30, 30));
    }

    private void Update()
    {
        if (building == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, pos, 0.1f);
            
            if (Vector3.Distance(transform.position, pos) < 1f)
            {
                building = true;
            }
        }

        if (building == true)
        {
            GetComponent<Rigidbody>().velocity = new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);
            RaycastHit hit;
            if (timer == false)
            {
                if (Physics.Raycast(transform.position + transform.up, -transform.up, out hit, 2.1f))
                {
                    GetComponent<Rigidbody>().AddForce(transform.up * 0.5f, ForceMode.Impulse);
                }
                else
                {
                    if (Physics.Raycast(transform.position + transform.up, -transform.up, out hit, 4f))
                    {
                        if (timer == false)
                        {
                            Instantiate(cubePrefab, new Vector3(hit.point.x, hit.point.y + cubePrefab.transform.localScale.y / 2, hit.point.z), Quaternion.identity);
                            timer = true;
                        }
                    }
                }
            }

            if (count >= 5)
            {
                building = false;
                pos = transform.position + new Vector3(Random.Range(-30, 30), 0, Random.Range(-30, 30));
                count = 0;
            }
        }
    }

    private void FixedUpdate()
    {
        if (timer == true)
        {
            start += 0.1f;
            if (start >= end)
            {
                timer = false;
                start = 0;
                count += 1;
            }
        }
    }
}
