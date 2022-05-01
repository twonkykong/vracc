using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flagZone : MonoBehaviour
{
    public bool command1, captured;
    public Transform redflagSpawn, blueflagSpawn, redSmallFlagSpawn, blueSmallFlagSpawn;

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "flag" + System.Convert.ToSingle(command1))
        {
            Debug.Log("flag of command " + System.Convert.ToSingle(command1) + " captured!!!!!!!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "flag" + System.Convert.ToSingle(!command1))
        {
            Debug.Log("flag of command " + other.tag.Replace("flag", "") + " really captured....");
            string command;
            if (other.tag == "flag0")
            {
                command = "blue";
                other.transform.position = blueflagSpawn.position;
            }
            else
            {
                command = "red";
                other.transform.position = blueflagSpawn.position;
            }
            GameObject.Find("Main Camera").GetComponent<captureflaggeneral>().captured(3, command);
        }
        else if (other.tag == "smallflag" + System.Convert.ToSingle(!command1))
        {
            Debug.Log("smallflag of command " + other.tag.Replace("flag", "") + " really captured....");
            string command;
            if (other.tag == "smallflag0")
            {
                command = "blue";
                other.transform.position = blueSmallFlagSpawn.position;
            }
            else
            {
                command = "red";
                other.transform.position = redSmallFlagSpawn.position;
            }
            GameObject.Find("Main Camera").GetComponent<captureflaggeneral>().captured(1, command);
        }
        else if (other.tag == "flag" + System.Convert.ToSingle(command1))
        {
            Debug.Log("flag of command " + System.Convert.ToSingle(command1) + " returned");
        }
    }
}
