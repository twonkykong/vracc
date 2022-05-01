using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generator : MonoBehaviour
{
    public List<GameObject> connectings;

    private void Update()
    {
        foreach(GameObject obj in connectings)
        {
            if (obj == null) connectings.Remove(obj);
        }
    }
}
