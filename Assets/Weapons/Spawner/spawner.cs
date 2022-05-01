using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class spawner : MonoBehaviour
{
    public GameObject selected, propMenu, player, effect;
    RaycastHit hit;
    private void Update()
    {
        //spawn
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Debug.DrawRay(transform.position, transform.forward, Color.yellow);
            if (Physics.Raycast(transform.position, transform.forward, out hit, 4f))
            {
                if (Input.GetMouseButtonDown(0)) spawnObj();

                if (hit.collider.tag == "prop")
                {
                    //delete
                    if (Input.GetMouseButtonDown(1)) deleteObj();

                    //duplicate
                    if (Input.GetMouseButtonDown(2)) duplicateObj();
                }
            }
        }
    }

    public void spawnObj()
    {
        GameObject g = PhotonNetwork.Instantiate(selected.name.Split(' ')[0].Split('(')[0], new Vector3(hit.point.x, hit.point.y + selected.transform.localScale.y / 2, hit.point.z), new Quaternion(0, transform.rotation.y, 0, transform.rotation.w));
        PhotonNetwork.Instantiate(effect.name, g.transform.position, g.transform.rotation);
    }

    public void deleteObj()
    {
        PhotonNetwork.Destroy(hit.collider.gameObject);
    }

    public void duplicateObj()
    {
        selected = hit.collider.gameObject;
    }
}
