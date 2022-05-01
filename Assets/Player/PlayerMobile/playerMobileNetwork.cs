using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class playerMobileNetwork : MonoBehaviourPunCallbacks
{
    public GameObject[] hide, show;
    public GameObject cam;
    private void Update()
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            foreach (GameObject obj in hide) obj.SetActive(false);
            foreach (GameObject obj in show) obj.SetActive(true);
            GetComponent<Player>().enabled = false;
            cam.GetComponent<Camera>().enabled = false;
        }
        
    }
}
