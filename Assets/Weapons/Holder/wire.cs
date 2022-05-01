using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;
using Photon.Pun;

public class wire : MonoBehaviourPun
{
    public GameObject generator, obj;
    public bool wiring;

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 4f))
        {
            if (hit.collider.gameObject.layer == 10)
            {
                obj = hit.collider.gameObject;
            }
            else obj = null;
        }
        else obj = null;
    }

    public void Wire()
    {
        if (!wiring)
        {
            if (obj.tag == "generator")
            {
                wiring = true;
                generator = obj;
            }
            else if (obj.GetComponent<technology>() != null)
            {
                obj.GetComponent<technology>().power(false);
            }
        }
        else
        {
            if (obj.GetComponent<technology>() != null && generator.GetComponent<generator>().connectings.Count < 5 && obj.GetComponent<technology>().work == false)
            {
                this.photonView.RPC("WireRPC", RpcTarget.AllBuffered, generator.GetComponent<PhotonView>().ViewID, obj.GetComponent<PhotonView>().ViewID);
            }
            wiring = false;
        }
    }

    [PunRPC]
    public void WireRPC(int generatorID, int techID)
    {
        GameObject generator = PhotonView.Find(generatorID).gameObject;
        GameObject tech = PhotonView.Find(techID).gameObject;

        tech.GetComponent<technology>().power(true);
        tech.GetComponent<technology>().generator = generator;
        generator.GetComponent<generator>().connectings.Add(tech);
    }
}
