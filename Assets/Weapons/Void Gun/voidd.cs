using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class voidd : MonoBehaviour
{
    public Collider[] col;

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 pos = transform.position;
        col = Physics.OverlapSphere(transform.position, 3f);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        foreach (Collider col in col)
        {
            transform.position = pos;
            if (col.gameObject.GetComponent<Rigidbody>() != null)
            {
                Vector3 dir = transform.position - col.transform.position;
                if (col.tag != "Player") col.gameObject.GetComponent<Rigidbody>().useGravity = false;
                col.gameObject.GetComponent<Rigidbody>().AddForce(dir * 10f, ForceMode.Impulse);
            }
        }
    }
}
