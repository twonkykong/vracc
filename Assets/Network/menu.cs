using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using System.Text.RegularExpressions;
using Photon.Pun.Demo.Asteroids;
using System.IO;

public class menu : MonoBehaviourPunCallbacks
{
    public GameObject btns, connectingText, btn, serverPrefab, servers, cam, canvas;
    public Text stats, maxplayerstext, onlinetext, senstext, friends, fovtext, roomsAvailable;
    public Slider maxplayersscroll, sens, fov;
    public string nickname;
    float start;
    bool checkServers;

    //body color
    public Image colorPrev;
    public Slider[] colors;
    public Text[] colorTexts;

    //room properties
    public Dropdown map;
    public Toggle isVisible;

    public List<GameObject> serversBtns;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (PlayerPrefs.GetString("nickname") == null || PlayerPrefs.GetString("nickname") == "") nickname = "Player " + Random.Range(0, 100);
        else nickname = PlayerPrefs.GetString("nickname");

        if (PlayerPrefs.GetInt("sens") < 30) PlayerPrefs.SetInt("sens", 100);
        sens.value = PlayerPrefs.GetInt("sens");

        if (PlayerPrefs.GetInt("fov") < 20) PlayerPrefs.SetInt("sens", 60);
        fov.value = PlayerPrefs.GetInt("fov");

        if (PlayerPrefs.GetString("bodyColor") == "" || PlayerPrefs.GetString("bodyColor") == null)
        {
            foreach(Slider sl in colors)
            {
                sl.value = 255;
            }

            PlayerPrefs.SetString("bodyColor", colors[0].value + "/" + colors[1].value + "/" + colors[2].value);
        }
        else
        {
            string[] colorstring = PlayerPrefs.GetString("bodyColor").Split('/');
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i].value = System.Convert.ToInt32(colorstring[i]);
            }
        }

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Update()
    {
        PhotonNetwork.NickName = nickname;
        stats.text = "\nNickname: " + PhotonNetwork.NickName + "\nRegion: " + PhotonNetwork.CloudRegion + "\nPlayers online: " + PhotonNetwork.CountOfPlayers + "\nRooms: " + PhotonNetwork.CountOfRooms;

        PlayerPrefs.SetInt("sens", System.Convert.ToInt32(sens.value));
        senstext.text = "" + sens.value;

        PlayerPrefs.SetInt("fov", System.Convert.ToInt32(fov.value));
        fovtext.text = "" + fov.value;

        cam.transform.Rotate(transform.up, -0.1f);

        //rooms available btns
        if (checkServers)
        {
            TypedLobby typedlobby = new TypedLobby("typedlobby", LobbyType.Default);
            PhotonNetwork.GetCustomRoomList(typedlobby, null);
        }

        //rooms available
        foreach (GameObject btn in serversBtns)
        {
            Transform children = btn.GetComponentInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (child.name == "players")
                {
                    if (child.GetComponent<Text>().text.Split('/')[0] == "0")
                    {
                        serversBtns.Remove(btn);
                        Destroy(btn);
                    }
                }
            }
        }

        roomsAvailable.text = "rooms available: " + PhotonNetwork.CountOfRooms;

        //rooms properties btns
        maxplayerstext.text = "max players: " + maxplayersscroll.value;

        //body color
        for (int i = 0; i < colors.Length; i++)
        {
            colorTexts[i].text = "" + colors[i].value;
        }
        colorPrev.color = new Color(colors[0].value / 255, colors[1].value / 255, colors[2].value / 255);
    }

    public void createroom(Text createText)
    {
        ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable();
        ht.Add("roomName", map.options[map.value].text);

        string roomname = "";
        if (createText.text != "") roomname = createText.text;
        else roomname = "Room " + PhotonNetwork.CountOfRooms;
        PhotonNetwork.CreateRoom(roomname, new Photon.Realtime.RoomOptions { MaxPlayers = System.Convert.ToByte(maxplayersscroll.value), IsVisible = isVisible.isOn, CustomRoomProperties = ht});
        Debug.Log(ht["roomName"]);
    }

    public void joinroom(Text joinText)
    {
        if (joinText.text !=  "") PhotonNetwork.JoinRoom(joinText.text);
        else PhotonNetwork.JoinRandomRoom();
    }

    public void leaveroom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void quit()
    {
        Application.Quit();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("created");
        PhotonNetwork.LoadLevel(map.options[map.value].text);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("joined");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("master");
        btns.SetActive(true);
        connectingText.SetActive(false);
        PhotonNetwork.JoinLobby();

        PlayerPrefs.SetString("userid", PhotonNetwork.AuthValues.UserId);
    }

    public override void OnConnected()
    {
        Debug.Log("connected " + PhotonNetwork.NickName);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("lobby");
        checkServers = true;
    }

    private void FixedUpdate()
    {
        start += 0.1f;
        if (start >= 50f) btn.SetActive(true);
    }

    public void changeNickname(InputField text)
    {
        if (text.text != "")
        {
            nickname = text.text;
            PlayerPrefs.SetString("nickname", nickname);
            text.text = "";
        }
    }

    public void changeSkin(string data)
    {
        PlayerPrefs.SetString(data.Split(' ')[0], data.Split(' ')[1]);
    }

    public void changeBodyColor()
    {
        PlayerPrefs.SetString("bodyColor", colors[0].value + "/" + colors[1].value + "/" + colors[2].value);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            Debug.Log(room);
            if (room.IsOpen == true && room.IsVisible == true)
            {
                if (GameObject.Find(room.Name) != null)
                {
                    serversBtns.Remove(GameObject.Find(room.Name));
                    Destroy(GameObject.Find(room.Name));
                }
                GameObject btn = Instantiate(serverPrefab, servers.transform);
                btn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 90 - (35 * serversBtns.Count));
                btn.name = room.Name;

                Transform children = btn.GetComponentInChildren<Transform>();
                foreach (Transform child in children)
                {
                    if (child.name == "name")
                    {
                        child.GetComponent<Text>().text = room.Name;
                        btn.GetComponent<Button>().onClick.AddListener(() => joinroom(child.GetComponent<Text>()));
                    }
                    if (child.name == "players") child.GetComponent<Text>().text = room.PlayerCount + "/" + room.MaxPlayers;
                }

                serversBtns.Add(btn);
            }
        }
    }
}
