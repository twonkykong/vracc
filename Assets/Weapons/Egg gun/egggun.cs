using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class egggun : MonoBehaviour
{
    public GameObject egg;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject g = Instantiate(egg, transform.position + transform.forward * 0.6f, Quaternion.identity);
            g.GetComponent<Rigidbody>().AddForce(transform.forward * 10f + transform.up * 2f, ForceMode.Impulse);
        }
    }
}
