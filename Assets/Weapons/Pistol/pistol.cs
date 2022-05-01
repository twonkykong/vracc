using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class pistol : MonoBehaviourPun
{
    public Animator gun;
    public GameObject player;

    float start;
    int ammo = 6;

    private void Update()
    {
        if (!GetComponent<PhotonView>().IsMine) enabled = false;
        if (gun.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        {
            if (ammo == 0)
            {
                gun.SetBool("reload", true);
            }

            if (Input.GetMouseButtonDown(0) && ammo > 0)
            {
                ammo -= 1;
                gun.SetBool("shot", true);
                this.photonView.RPC("shotSound", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, 50f))
                {
                    GameObject obj = hit.collider.gameObject;
                    while (obj.transform.parent != null)
                    {
                        obj = obj.transform.parent.gameObject;
                    }

                    if (obj.tag == "Player")
                    {
                        print(obj.GetComponent<PhotonView>().Owner.NickName);
                        this.photonView.RPC("getDamage", RpcTarget.All);
                    }

                    if (obj.layer == 10)
                    {
                        obj.GetComponent<Rigidbody>().AddForce(transform.forward * 10f, ForceMode.Impulse);
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (gun.GetBool("shot"))
        {
            start += 0.1f;
            if (start >= 1)
            {
                gun.SetBool("shot", false);
                start = 0;
            }
        }

        if (gun.GetBool("reload"))
        {
            start += 0.1f;
            if (start >= 4)
            {
                gun.SetBool("reload", false);
                start = 0;
                ammo = 6;
            }
        }
    }

    [PunRPC]
    public void shotSound(string nickname)
    {
        foreach (GameObject playerObj in GameObject.FindObjectsOfType<GameObject>().Where(playerObj => playerObj.name == "PlayerPC 1 (Clone)"))
        {
            if (playerObj.GetComponent<PhotonView>().Owner.NickName == nickname)
            {
                foreach (Transform child in playerObj.GetComponentInChildren<Transform>())
                {
                    if (child.gameObject.name == "revolver") child.GetComponent<AudioSource>().Play();
                }
            }
        }
    }

    [PunRPC]
    public void getDamage()
    {
        GameObject parent = transform.parent.gameObject;
        while (parent.transform.parent != null) parent = parent.transform.parent.gameObject;
        if (parent.GetComponent<PhotonView>().IsMine) parent.GetComponent<Player>().hp -= 30;
    }
}
