using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using Photon.Voice.Unity.UtilityScripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviourPun
{
    public float hp = 100, start, start1, end, velocityY, animPlayTimer;
    public Text hpText;
    public bool menu, cursorLock, pc, dead, spectating;
    public GameObject[] playerAlive;
    public GameObject playerRagdollPrefab, cam, cam2, inGame, menubtns, gameManager, head, body;
    public Quaternion rot;
    GameObject ragdoll;
    public Text messagesText, statsText, playersCount, playerNickname;

    public List<string> messages;

    bool grounded, spawned;

    private void Start()
    {
        if (!GetComponent<PhotonView>().IsMine) return;

        GetComponent<Animator>().Play("claps");

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) pc = false;
        else pc = true;

        gameManager = GameObject.Find("game manager");

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (PhotonNetwork.LocalPlayer.NickName == player.NickName && !player.IsLocal)
            {
                PhotonNetwork.LocalPlayer.NickName += "_1";
            }
        }

        //body color
        this.photonView.RPC("customizePlayer", RpcTarget.AllBuffered, PlayerPrefs.GetString("bodyColor"), this.photonView.ViewID);
        body.layer = 11;
    }

    private void Update()
    {
        if (!GetComponent<PhotonView>().IsMine) return;

        if (Physics.Raycast(transform.position + transform.up, -transform.up, 1.3f)) grounded = true;
        else grounded = false;

        velocityY = GetComponent<Rigidbody>().velocity.y;

        //hp
        hpText.text = "HP: " + hp;
        if (hp <= 0) dead = true;

        //pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pause();
            openMenu();
        }

        if (cursorLock == true)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            if (pc == true)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        //network
        statsText.text = PhotonNetwork.CurrentRoom.Name + "\n" + PhotonNetwork.NickName;
        if (PhotonNetwork.IsMasterClient) statsText.text += "\nYou are owner of this room!";

        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
        playersCount.text = "Players (" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + ")";
        foreach (Photon.Realtime.Player player in players) playersCount.text += "\n" + player.NickName;

        //player nickname
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 4f))
        {
            if (hit.collider.GetComponent<Player>() != null)
            {
                playerNickname.text = hit.collider.GetComponent<PhotonView>().Owner.NickName;
            }
            else playerNickname.text = "";
        }
        else playerNickname.text = "";

        //chat
        if (Input.GetKeyDown(KeyCode.Return)) pause();

        //pause menu
        if (menu == true)
        {
            inGame.SetActive(false);
            menubtns.SetActive(true);
        }
        else
        {
            if (dead == false) inGame.SetActive(true);
            else inGame.SetActive(false);
            menubtns.SetActive(false);
        }

        //messages
        if (messagesText.text.Split('\n').Length > 8) gameManager.GetComponent<game>().messages.RemoveAt(0);
        Debug.Log(messagesText.text.Split('\n'));

        messages = gameManager.GetComponent<game>().messages;
        messagesText.text = "";
        foreach (string message in messages) messagesText.text += "\n" + message;
    }

    public void pause()
    {
        if (cursorLock == false)
        {
            //if (cam.GetComponent<holderSpawner>().customizingObj == true) cam.GetComponent<holderSpawner>().photonView.RPC("customObj", RpcTarget.AllBuffered, new float[3] { cam.GetComponent<holderSpawner>().colors[0].value, cam.GetComponent<holderSpawner>().colors[1].value, cam.GetComponent<holderSpawner>().colors[2].value }, cam.GetComponent<holderSpawner>().gravityToggle.isOn, cam.GetComponent<holderSpawner>().size.value);
            //else
            //{
                rot = transform.rotation;
                cursorLock = true;
            //}
        }
        else cursorLock = false;
    }

    public void openMenu()
    {
        if (menu == false)
        {
            //if (cam.GetComponent<holderSpawner>().customizingObj) cam.GetComponent<holderSpawner>().openCustomsObj();
            //else menu = true;
        }
        else menu = false;
    }

    private void FixedUpdate()
    {
        if (dead == true)
        {
            foreach (GameObject obj in playerAlive) obj.SetActive(false);
            if (spawned == false)
            {
                ragdoll = PhotonNetwork.Instantiate(playerRagdollPrefab.name, transform.position, transform.rotation);
                spawned = true;
            }
            cam.transform.LookAt(ragdoll.transform.position);
            cursorLock = true;
            if (!spectating) start += 0.1f;
            if (start >= end)
            {
                cursorLock = false;
                foreach (GameObject obj in playerAlive) obj.SetActive(true);
                transform.position = new Vector3(UnityEngine.Random.Range(-5, 5), 0, UnityEngine.Random.Range(-5, 5));
                hp = 100;
                dead = false;
                start = 0;
                PhotonNetwork.Destroy(ragdoll);
                spawned = false;
                cam.transform.rotation = transform.rotation;
            }
        }

        if (hp < 100 && dead == false)
        {
            start1 += 0.1f;
            if (start1 >= 5)
            {
                if (hp <= 25) hp += 4;
                else if (hp <= 75) hp += 2;
                else if (hp <= 100) hp += 1f;
                start1 = 0;
            }
        }
    }

    public void sendMessages(InputField input)
    {
        gameManager.GetComponent<game>().sendMessage(input, gameObject);
        if (pc == true) pause();
    }

    [PunRPC]
    public void SetParent(int objID, int parentID)
    {
        GameObject obj = PhotonView.Find(objID).gameObject;
        GameObject parent = PhotonView.Find(parentID).gameObject;
        obj.transform.SetParent(parent.transform);
    }

    public void GetDamage(int damage)
    {
        hp -= damage;
    }

    [PunRPC]
    public void customizePlayer(string bodyColor, int viewID)
    {
        string[] colorstr = bodyColor.Split('/');
        PhotonView.Find(viewID).gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = new Color(System.Convert.ToInt32(colorstr[0]) / 255, System.Convert.ToInt32(colorstr[1]) / 255, System.Convert.ToInt32(colorstr[2]) / 255);
    }
}
