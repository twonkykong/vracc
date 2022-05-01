using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class buttonServer : MonoBehaviour
{
    public Text name, players;

    public void SetName(RoomInfo room)
    {
        name.text = room.Name;
        players.text = room.PlayerCount + "/" + room.MaxPlayers;
    }
}
