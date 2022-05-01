using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showHideBtn : MonoBehaviour
{
    public GameObject[] show, hide;

    public void press()
    {
        foreach (GameObject show in show) show.SetActive(true);
        foreach (GameObject hide in hide) hide.SetActive(false);
    }
}
