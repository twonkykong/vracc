using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class captureflaggeneral : MonoBehaviour
{
    float start;
    bool play;
    public int scoreRed, scoreBlue, red, blue;
    public GameObject door;

    private void Start()
    {
        GetComponent<game>().captureflag = true;
    }

    [PunRPC]
    public void captured(int scoreAdd, string side)
    {
        if (side == "red") scoreRed += scoreAdd;
        else scoreBlue += scoreAdd;
    }

    private void FixedUpdate()
    {
        if (play == false)
        {
            start += 0.1f;
            if (start >= 25)
            {
                door.GetComponent<Animation>().Play();
                play = true;
            }
        }
    }
}
