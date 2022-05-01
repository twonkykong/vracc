using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Pun.Demo.Procedural;

public class game : MonoBehaviourPunCallbacks
{
    public GameObject playerPC, playerMobile, lava;

    public List<string> messages;

    string time;

    string[] deadText;

    bool foundPlayer, localmsg, mp, pc;

    public bool captureflag, lavaRising, lavaGoesDown;
    public int command;
    public float start, start1, start2;

    Vector3 pos;

    private void Awake()
    {
        Message("(" + System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute + ") Server : server started");
    }

    private void Start()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) pc = false;
        else pc = true;

        if (pc == true) PhotonNetwork.Instantiate(playerPC.name, pos, Quaternion.identity);
        else PhotonNetwork.Instantiate(playerMobile.name, pos, Quaternion.identity);

        deadText = new string[5] { " had a bad day.", " killed himself.", " unfortunately died.", " went to heaven.", " decided to respawn." };
    }
    public void leave()
    {
        GameObject[] props = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject prop in props) 
        {
            if (prop.layer == 10) 
            {
                if (prop.GetComponent<PhotonView>().IsMine)
                {
                    prop.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.PlayerList[Random.Range(0, PhotonNetwork.PlayerList.Length)]);
                }
            }
        }
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("menu");
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("Entered room " + newPlayer.NickName);
        Message("(" + System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute + ") Server : " + newPlayer.NickName + " entered room.");
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log("Left room " + otherPlayer.NickName);
        Message("(" + System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute + ") Server : " + otherPlayer.NickName + " left room.");
    }

    public void sendMessage(InputField input, GameObject thisPlayer)
    {
        if (input.text != "")
        {
            string message = "";
            if (input.text.Split(' ')[0] == "/me") message = "Server : " + PhotonNetwork.NickName + " " + input.text.Replace("/me ", "");
            else if (input.text == "/kill")
            {
                message = "Server : " + PhotonNetwork.NickName + deadText[Random.Range(0, 6)];
                localmsg = false;
                thisPlayer.GetComponent<Player>().dead = true;
            }
            else if (input.text.Split(' ')[0] == "/clear")
            {
                if (input.text.Split(' ').Length == 2 && input.text.Split(' ')[1] != "")
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
                        {
                            if (player.NickName == input.text.Split(' ')[1])
                            {
                                foundPlayer = true;

                                int propsCount = 0;
                                GameObject[] props = GameObject.FindObjectsOfType<GameObject>();
                                foreach (GameObject prop in props)
                                {
                                    if (prop.layer == 10)
                                    {
                                        if (prop.GetComponent<PhotonView>().Owner.UserId == player.UserId) propsCount += 1;
                                        prop.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
                                        PhotonNetwork.Destroy(prop);
                                    }
                                }
                                message = "Server : cleared " + propsCount + " " + player.NickName + "'s props.";
                            }
                        }
                        if (foundPlayer == false)
                        {
                            message = "Server : unable to find " + input.text.Split(' ')[1] + " on this server.";
                            localmsg = true;
                        }
                    }
                    else
                    {
                        message = "Server : permission denied.";
                        localmsg = false;
                    }
                }
                else
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        if (mp == true) this.photonView.RPC("clearMap", RpcTarget.All);
                        else clearMap();
                        message = "Server : cleared " + GameObject.FindGameObjectsWithTag("prop").Length + " props.";
                    }
                    else
                    {
                        int propsCount = 0;
                        GameObject[] props = GameObject.FindObjectsOfType<GameObject>();
                        foreach (GameObject prop in props)
                        {
                            if (prop.layer == 10)
                            {
                                if (prop.GetComponent<PhotonView>().Owner.UserId == thisPlayer.GetComponent<PhotonView>().Owner.NickName) propsCount += 1;
                                prop.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
                                PhotonNetwork.Destroy(prop);
                            }
                        }
                        message = "Server : cleared " + props + " " + thisPlayer.GetComponent<PhotonView>().Owner.NickName + "'s props.";
                    }
                }
            }
            else if (input.text.Split(' ')[0] == "/tp")
            {
                if (input.text.Split(' ').Length == 4)
                {
                    thisPlayer.transform.position = new Vector3(System.Convert.ToInt32(input.text.Split(' ')[1]), System.Convert.ToInt32(input.text.Split(' ')[2]), System.Convert.ToInt32(input.text.Split(' ')[3]));
                    message = "Server : teleported " + PhotonNetwork.NickName + " to (" + input.text.Split(' ')[1] + ", " + input.text.Split(' ')[2] + ", " + input.text.Split(' ')[3] + ").";
                }
                else
                {
                    message = "Server : not enough arguments.";
                    localmsg = true;
                }
            }
            else if (input.text == "/cords")
            {
                message = "Server : your cords are: " + thisPlayer.transform.position;
                localmsg = true;
            }
            else if (input.text == "/help")
            {
                message = "Server : /me - roleplay chat\n/kill - kill yourself\n/cords - show cords\n/tp x y z - teleport to given cords (x, y, z)\n/help - commands help\n\nfor admin:\n/clear player_nickname - delete all props of player_nickname or all props on the server, if player_nickname argument is not given.";
                localmsg = true;
                foreach (string msg in messages) messages.Remove(msg);
            }
            else
            {
                message = PhotonNetwork.NickName + " : " + input.text;
                localmsg = false;
            }

            if (localmsg) Message("(" + System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute + ") " + message);
            else this.photonView.RPC("Message", RpcTarget.All, "(" + System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute + ") " + message);
            input.text = "";
        }
    }

    [PunRPC]
    public void Message(string message)
    {
        if (messages.Count > 8) messages.RemoveAt(0);
        messages.Add(message);
    }

    [PunRPC]
    public void clearMap()
    {
        GameObject[] props = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject prop in props)
        {
            if (prop.layer == 10)
            {
                Debug.Log(prop.name);
                prop.GetComponent<PhotonView>().TransferOwnership(GetComponent<PhotonView>().Owner);
                PhotonNetwork.Destroy(prop);
            }
        }
    }
}
