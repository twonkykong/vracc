using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class deleteByTime : MonoBehaviour
{
    public float end;
    float start;
    public bool startTimer;

    private void OnCollisionEnter(Collision collision)
    {
        startTimer = true;
    }
    private void FixedUpdate()
    {
        if (startTimer)
        {
            start += 0.1f;
            if (start >= end) PhotonNetwork.Destroy(gameObject);
        }
    }
}
