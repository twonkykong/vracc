using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deathTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.transform.position = new Vector3(-14, 5, -30);
        }

        if (other.tag == "prop")
        {
            Destroy(other.gameObject);
        }
    }
}
