using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weld : MonoBehaviourPun
{
    public GameObject parentPrefab, obj, firstObj, secondObj;
    public bool welding;
    void Update()
    {
        //am i looking at prop
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 4f))
        {
            if (hit.collider.gameObject.layer == 10)
            {
                obj = hit.collider.gameObject;
                if (obj.tag != "ragdoll" && obj.transform.parent != null)
                {
                    while (obj.transform.parent != null)
                    {
                        obj = obj.transform.parent.gameObject;
                    }
                }

                if (welding) secondObj = obj;
            }
            else obj = null;
        }
        else obj = null;
        

        //pc inputs
        if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
        {
            if (!welding)
            {
                if (Input.GetKeyDown(KeyCode.G)) Weld();
            }
            if (welding)
            {
                if (Input.GetMouseButtonDown(0)) Weld("weld");
                if (Input.GetMouseButtonDown(1)) Weld("rope");
            }
        }
    }

    public void Weld(string type = null)
    {
        if (welding == false)
        {
            firstObj = obj;
            GetComponent<holderSpawner>().enabled = false;
            welding = true;
        }
        else
        {
            GameObject parentObj = PhotonNetwork.Instantiate(parentPrefab.name, (firstObj.transform.position - secondObj.transform.position)/2, Quaternion.identity);
            this.photonView.RPC("WeldRPC", RpcTarget.AllBuffered, type, firstObj.GetComponent<PhotonView>().ViewID, secondObj.GetComponent<PhotonView>().ViewID, parentObj.GetComponent<PhotonView>().ViewID);
            GetComponent<holderSpawner>().enabled = true;
            welding = false;
        }
    }
    
    [PunRPC]
    public void WeldRPC(string type, int firstObjID, int secondObjID, int parentObjID)
    {
        GameObject firstObj = PhotonView.Find(firstObjID).gameObject;
        GameObject secondObj = PhotonView.Find(secondObjID).gameObject;
        GameObject parentObj = PhotonView.Find(parentObjID).gameObject;
        if (type == "weld")
        {
            Destroy(firstObj.GetComponent<Rigidbody>());
            Destroy(secondObj.GetComponent<Rigidbody>());
            firstObj.transform.parent = parentObj.transform;
            secondObj.transform.parent = parentObj.transform;
        }
        else if (type == "rope")
        {
            parentObj.AddComponent<BoxCollider>();
            firstObj.AddComponent<SpringJoint>();
            firstObj.GetComponent<SpringJoint>().connectedBody = parentObj.GetComponent<Rigidbody>();

            parentObj.AddComponent<SpringJoint>();
            parentObj.GetComponent<SpringJoint>().connectedBody = secondObj.GetComponent<Rigidbody>();

            parentObj.AddComponent<rope>();
            parentObj.GetComponent<rope>().targets = new GameObject[2] { firstObj, secondObj };
        }
    }
}
