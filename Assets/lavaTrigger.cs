using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lavaTrigger : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Rigidbody>() != null)
        {
            other.GetComponent<Rigidbody>().velocity += transform.up;
            if (other.GetComponent<Rigidbody>().velocity.y >= 2) other.GetComponent<Rigidbody>().velocity = new Vector3(other.GetComponent<Rigidbody>().velocity.x, 2, other.GetComponent<Rigidbody>().velocity.z);
        }
        if (other.tag == "Player")
        {
            other.GetComponent<Player>().hp -= 2;
        }
    }
}
