using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Security.Cryptography;
using UnityEditor.Experimental.UIElements.GraphView;

public class holderSpawner : MonoBehaviourPunCallbacks
{
    public GameObject selected, holdPoint, propMenu, inGame, colorPicker, player, spawnEffect, deleteEffect, buttonPrefab, additionalContent, obj, customizeBtn, selectedPreview, vehicleButton, wireButton;

    public GameObject[] btnsHide, btnsShow;
    public RaycastHit hit;
    public Text selectedText, sizeText;
    public Animator anim;
    public Slider[] colors;
    public Slider size;
    public Text[] colorsTexts;
    public Toggle gravityToggle;
    public bool customizingObj, holding, wireConnecting;

    public string[] files;

    bool interactiveTimer, grid, pickedRot, menu, welding;
    Material mat;
    float objRotYDif, distance, start;
    Vector3 difference;
    Quaternion rot;
    GameObject holdPoint;

    //wire
    public GameObject generator;

    private void Update()
    {
        //animation
        if (holding == true) anim.SetBool("hold", true);
        else anim.SetBool("hold", false);

        if (player.GetComponent<Player>().cursorLock == false)
        {
            //am i looking at prop
            if (!welding)
            {
                if (Physics.Raycast(transform.position, transform.forward, out hit, 4f))
                {
                    if (holding == false)
                    {
                        //spawn
                        if (player.GetComponent<Player>().pc == true) if (Input.GetMouseButtonDown(0)) spawnObj();

                        if (hit.collider.gameObject.layer == 10)
                        {
                            distance = Vector3.Distance(transform.position, hit.collider.transform.position);
                            if (hit.collider.transform.parent != null && hit.collider.tag != "ragdoll")
                            {
                                obj = hit.collider.transform.parent.gameObject;
                                while (obj.transform.parent != null) obj = obj.transform.parent.gameObject;
                            }
                            else obj = hit.collider.gameObject;

                            if (holdPoint == null)
                            {
                                GameObject a = new GameObject();
                                holdPoint = Instantiate(a, hit.point, Quaternion.identity);
                                holdPoint.transform.SetParent(gameObject.transform);
                            }
                            holdPoint.transform.position = hit.point;

                            if (wireConnecting == false)
                            {
                                //delete
                                if (player.GetComponent<Player>().pc == true) if (Input.GetMouseButtonDown(1)) deleteObj();

                                //duplicate
                                if (player.GetComponent<Player>().pc == true) if (Input.GetMouseButtonDown(2)) duplicateObj();

                                //color
                                if (player.GetComponent<Player>().pc == true && customizingObj == false) if (Input.GetKeyDown(KeyCode.C)) openCustomsObj();
                            }

                            //wire
                            if (player.GetComponent<Player>().pc == false)
                            {
                                customizeBtn.SetActive(true);
                                Debug.Log(obj);
                                if (obj.GetComponent<generator>() != null || (obj.GetComponent<technology>() != null && (wireConnecting == true || (wireConnecting == false && obj.GetComponent<technology>().work == true))))
                                {
                                    wireButton.SetActive(true);
                                }
                                else wireButton.SetActive(false);
                            }
                        }
                        else
                        {
                            obj = null;
                            if (player.GetComponent<Player>().pc == false)
                            {
                                vehicleButton.SetActive(false);
                                wireButton.SetActive(false);
                            }
                        }
                    }
                }
                else if (holding == false && customizingObj == false)
                {
                    obj = null;
                    if (player.GetComponent<Player>().pc == false) vehicleButton.SetActive(false);
                    wireButton.SetActive(false);
                }

                if (holding == true)
                {
                    if (player.GetComponent<Player>().pc == false)
                    {
                        customizeBtn.SetActive(false);
                        wireButton.SetActive(false);
                    }
                }

                if (obj != null)
                {
                    if (player.GetComponent<Player>().pc == true) if (Input.GetKeyDown(KeyCode.E)) hold();

                    if (holding == true)
                    {
                        obj.transform.rotation = Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y + transform.eulerAngles.y - minusRoty, rot.eulerAngles.z);

                        if (player.GetComponent<Player>().pc == true) if (Input.GetMouseButtonDown(0)) drop();

                        if (player.GetComponent<Player>().pc == true) if (Input.GetMouseButtonDown(1))
                            {
                                gravityOff();
                                hold();
                            }

                        if (player.GetComponent<Player>().pc == true) if (Input.GetKeyDown(KeyCode.R)) gridObj();

                        if (grid == true)
                        {
                            Vector3 pos = new Vector3(Mathf.Round(obj.transform.position.x), Mathf.Round(obj.transform.position.y), Mathf.Round(obj.transform.position.z));
                            Debug.Log(pos);
                            obj.transform.position = pos;
                            obj.transform.rotation = new Quaternion(obj.transform.rotation.x, 0, obj.transform.rotation.z, obj.transform.rotation.w);
                        }

                        foreach (GameObject hide in btnsHide) hide.SetActive(false);
                        foreach (GameObject show in btnsShow) show.SetActive(true);
                        if (wireButton != null) wireButton.SetActive(false);

                        if (obj == null) holding = false;
                        else if (!obj.GetComponent<PhotonView>().IsMine) holding = false;
                    }
                    else
                    {
                        foreach (GameObject hide in btnsHide) hide.SetActive(true);
                        foreach (GameObject show in btnsShow) show.SetActive(false);
                    }
                }
                else
                {
                    Destroy(holdPoint);
                    if (player.GetComponent<Player>().pc == false) customizeBtn.SetActive(false);
                }
            }
        }

        //open menu
        if (player.GetComponent<Player>().pc == true) if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyUp(KeyCode.Q)) openMenu();

        if (menu == true)
        {
            propMenu.SetActive(true);
            inGame.SetActive(false);

            //selected text
            selectedText.text = "selected: " + selected.name.Replace("_prop", "");
        }
        else
        {
            propMenu.SetActive(false);
            if (customizingObj == false) inGame.SetActive(true);
            else inGame.SetActive(false);
        }

        //customizeObj
        if (customizingObj)
        {
            for (int i = 0; i < colorsTexts.Length; i++) colorsTexts[i].text = "" + colors[i].value;
            sizeText.text = "" + size.value;

            if (obj.GetComponent<MeshRenderer>() != null) obj.GetComponent<MeshRenderer>().material.color = new Color(colors[0].value / 255, colors[1].value / 255, colors[2].value / 255);
            if (obj.GetComponentInChildren<Transform>() != null)
            {
                Transform children = obj.GetComponentInChildren<Transform>();
                foreach (Transform child in children) if (child.GetComponent<MeshRenderer>() != null) child.GetComponent<MeshRenderer>().material.color = new Color(colors[0].value / 255, colors[1].value / 255, colors[2].value / 255);
            }

            //size
            obj.transform.localScale = new Vector3(size.value, size.value, size.value);
        }

        if (player.GetComponent<Player>().pc == true)
        {
            //welding
            if (welding)
            {
                if (Input.GetMouseButtonDown(0)) weld("weld");
                else if (Input.GetMouseButtonDown(1)) weld("rope");
            }

            //wire connecting
            if (!welding && !holding && !customizingObj)
            {
                if (Input.GetKeyDown(KeyCode.G)) wire();
            }
        }

        if (wireConnecting)
        {
            if (player.GetComponent<Player>().pc == false)
            {
                foreach (GameObject hide in btnsHide) hide.SetActive(false);
                customizeBtn.SetActive(false);
            }
        }
    }

    public void Hold(string type = null)
    { 
        if (!holding)
        {
            obj.GetComponent<Rigidbody>().isKinematic = true;
            objRotYDif = transform.parent.parent.eulerAngles.y - obj.transform.eulerAngles.y;
            holding = true;
        }
        else
        {
            if (type != "freeze")
            {
                obj.GetComponent<Rigidbody>().isKinematic = false;
                if (type == "throw") obj.GetComponent<Rigidbody>().AddForce(transform.forward * 7f, ForceMode.Impulse);
            }
        }
    }

    public void spawnObj()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, 4f))
        {
            if (holding == false)
            {
                interactiveTimer = true;
                GameObject g = PhotonNetwork.Instantiate(selected.name.Split(' ')[0].Split('(')[0], new Vector3(hit.point.x, hit.point.y + selected.transform.localScale.y / 2, hit.point.z), new Quaternion(0, transform.rotation.y, 0, transform.rotation.w));
                PhotonNetwork.Instantiate(spawnEffect.name, g.transform.position, g.transform.rotation);
            }
        }
    }

    public void deleteObj()
    {
        if (holding == false && obj != null)
        {
            obj.GetComponent<PhotonView>().TransferOwnership(GetComponent<PhotonView>().Owner);
            PhotonNetwork.Instantiate(deleteEffect.name, hit.point, Quaternion.identity);
            PhotonNetwork.Destroy(obj);
            interactiveTimer = true;
        }
    }

    public void duplicateObj()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, 4f))
        {
            if (holding == false)
            {
                if (hit.collider.gameObject.layer == 10)
                {
                    selected = Resources.Load(hit.collider.gameObject.name.Split('(')[0]) as GameObject;
                }
            }
        }
    }

    public void gridObj()
    {
        if (grid == true) grid = false;
        else grid = true;
    }

    public void openCustomsObj()
    {
        if (customizingObj == false)
        {
            customizingObj = true;

            player.GetComponent<Player>().cursorLock = true;
            colorPicker.SetActive(true);
            if (obj.GetComponent<MeshRenderer>() != null) mat = obj.GetComponent<MeshRenderer>().material;
            else mat = obj.GetComponentInChildren<MeshRenderer>().material;

            gravityToggle.isOn = obj.GetComponent<Rigidbody>().useGravity;
            size.value = obj.transform.localScale.x;
            size.maxValue = obj.transform.localScale.x * 2;
            size.minValue = obj.transform.localScale.x / 2;

            colors[0].value = Mathf.Round(mat.color.r * 255);
            colors[1].value = Mathf.Round(mat.color.g * 255);
            colors[2].value = Mathf.Round(mat.color.b * 255);
        }
        else
        {
            customizingObj = false;
            this.photonView.RPC("customObj", RpcTarget.AllBuffered, new float[3] { colors[0].value, colors[1].value, colors[2].value }, gravityToggle.isOn, size.value);
            player.GetComponent<Player>().cursorLock = false;
            colorPicker.SetActive(false);
        }
    }

    public void openMenu()
    {
        if (menu == false)
        {
            if (customizingObj) openCustomsObj();
            menu = true;
            player.GetComponent<Player>().cursorLock = true;
        }
        else
        {
            menu = false;
            player.GetComponent<Player>().cursorLock = false;
        }
    }

    [PunRPC]
    public void customObj(float[] colors, bool gravity, float size)
    {
        if (obj.GetComponent<MeshRenderer>() != null) obj.GetComponent<MeshRenderer>().material.color = new Color(colors[0] / 255, colors[1] / 255, colors[2] / 255);
        else
        {
            Transform children = obj.GetComponentInChildren<Transform>();
            foreach (Transform child in children) if (child.GetComponent<MeshRenderer>() != null) child.GetComponent<MeshRenderer>().material.color = new Color(colors[0] / 255, colors[1] / 255, colors[2] / 255);
        }

        obj.GetComponent<Rigidbody>().useGravity = gravity;
        obj.transform.localScale = new Vector3(size, size, size);
    }

    private void FixedUpdate()
    {
        if (interactiveTimer)
        {
            anim.SetBool("interactive", true);
            start += 0.1f;
            if (start >= 1)
            {
                interactiveTimer = false;
                anim.SetBool("interactive", false);
                start = 0;
            }
        }
    }
}
