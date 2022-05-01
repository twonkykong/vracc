using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plane : MonoBehaviour
{
    public float speed = 0.5f;

    public GameObject propeller;

    private void Update()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, speed);
        propeller.transform.Rotate(0, 15, 0, Space.Self);

        if (Input.GetKeyDown(KeyCode.Space)) speed = 0.2f;
        else if (Input.GetKeyUp(KeyCode.Space)) speed = 0.5f;

        if (Input.GetKey(KeyCode.A)) rotate("y", -2);
        if (Input.GetKey(KeyCode.D)) rotate("y", 2);

        if (Input.GetKey(KeyCode.S)) rotate("x", 2);
        if (Input.GetKey(KeyCode.W)) rotate("x", -2);

        if (transform.rotation.x >= -90) transform.Rotate(0.1f, 0, 0, Space.Self);
    }

    public void rotate(string axis, float value)
    {
        if (axis == "y") transform.Rotate(0, value, 0, Space.Self);
        else if (axis == "x") transform.Rotate(value, 0, 0, Space.Self);
    }
}
