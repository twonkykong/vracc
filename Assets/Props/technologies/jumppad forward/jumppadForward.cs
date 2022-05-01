using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jumppadForward : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        GameObject obj = collision.collider.gameObject;
        if (obj.transform.parent != null) obj = obj.transform.parent.gameObject;

        if (obj.GetComponent<Rigidbody>() != null)
        {
            obj.GetComponent<Rigidbody>().AddForce((transform.up + transform.forward) * 5 / obj.GetComponent<Rigidbody>().mass, ForceMode.Impulse);
            obj = null;
        }
    }
}
