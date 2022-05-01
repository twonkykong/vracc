using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rope : MonoBehaviour
{
    public GameObject[] targets;

    public void Start()
    {
        gameObject.AddComponent<LineRenderer>();
        GetComponent<LineRenderer>().positionCount = 2;
        GetComponent<LineRenderer>().startWidth = 0.2f;
    }
    private void FixedUpdate()
    {
        GetComponent<LineRenderer>().SetPositions(new Vector3[2] { targets[0].transform.position, targets[1].transform.position });
    }
}
