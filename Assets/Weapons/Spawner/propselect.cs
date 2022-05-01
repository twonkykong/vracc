using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class propselect : MonoBehaviour
{
    public GameObject spawner, prop;

    public void pressed()
    {
        spawner.GetComponent<holderSpawner>().selected = prop;
    }
}
