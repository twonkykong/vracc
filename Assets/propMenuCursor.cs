using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class propMenuCursor : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(Input.mousePosition);
    }
}
