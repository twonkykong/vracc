using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skateboard : MonoBehaviour
{
    private void Update()
    {
        Debug.DrawRay(transform.position, transform.right, Color.red);
    }
    public void ollie()
    {
        transform.Rotate(40, 0, 0, Space.Self);
    }
}
