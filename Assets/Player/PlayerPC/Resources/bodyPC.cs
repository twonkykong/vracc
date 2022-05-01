using UnityEngine;
using System;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class bodyPC : MonoBehaviourPun
{

	public float speed = 5.0f;
	public float maxVelocityChange = 10.0f;
	public Rigidbody rb;

	public GameObject cam, head, cam2, cam2holder, headSkin, hand, gameManager, vehicle, body;
	public GameObject[] weaponsList;
	public float sensitivity = 100, jumpForce, mousewheel;
	public bool riding;
	public Text weaponsText;
	public Vector3 lookat, pos;
	public Animator anim;

	bool jumpTimer, grounded, crouching, runing;
	float start, start1, mousex, mousey;
	RaycastHit jumpHit;

	private void Start()
	{
		if (!GetComponent<PhotonView>().IsMine) return;

		rb = GetComponent<Rigidbody>();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private void Update()
	{
		if (!GetComponent<PhotonView>().IsMine) return;

		sensitivity = PlayerPrefs.GetInt("sens");
		cam.GetComponent<Camera>().fieldOfView = PlayerPrefs.GetInt("fov");

		if (GetComponent<Player>().cursorLock == false)
		{
			//animations
			if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) anim.SetBool("walk", true);
			else anim.SetBool("walk", false);

			//head rotation
			mousey += Input.GetAxis("Mouse Y") * sensitivity / 10;
			mousex += Input.GetAxis("Mouse X") * sensitivity / 10;

			if (mousey >= 89.9f) mousey = 89.9f;
			if (mousey <= -89.9f) mousey = -89.9f;
			if (mousex >= 360 || mousex <= -360) mousex = 0;

			headSkin.transform.eulerAngles = head.transform.eulerAngles = new Vector3(-mousey, head.transform.eulerAngles.y, head.transform.eulerAngles.z);
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, mousex, transform.eulerAngles.z);


			//camera
			RaycastHit hit1;
			int layermask = 1 << 2;
			layermask = ~layermask;
			if (Physics.Linecast(head.transform.position, cam2holder.transform.position, out hit1, layermask))
			{
				cam2.transform.position = hit1.point + transform.forward * 0.5f;
			}
			else cam2.transform.position = cam2holder.transform.position;

			//jump
			if (Physics.Raycast(transform.position + transform.up, -transform.up, out jumpHit, 1.3f)) grounded = true;
			else grounded = false;

			//some useable things
			RaycastHit hit;
			if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 4f))
			{
				//vehicles
				if (hit.collider.tag == "vehicle" && riding == false) vehicle = hit.collider.transform.parent.gameObject;
			}
			else
			{
				if (riding == false) vehicle = null;
			}

			//weapons change
			mousewheel += Input.GetAxis("Mouse ScrollWheel");
			if (mousewheel >= weaponsList.Length - 1) mousewheel = 0;
			else if (mousewheel <= 0) mousewheel = weaponsList.Length - 1;

			foreach (GameObject weapon in weaponsList)
			{
				weapon.SetActive(false);
				if (weaponsList[Convert.ToInt32(mousewheel)] == weapon)
				{
					weapon.SetActive(true);
					weaponsText.text = weapon.name;
				}
			}

			if (Input.GetKeyDown(KeyCode.Space)) jump();
			if (Input.GetKeyDown(KeyCode.F)) vehicleEnter();
			if (Input.GetKeyDown(KeyCode.LeftControl)) this.photonView.RPC("crouch", RpcTarget.AllBuffered, 9, 12, this.photonView.ViewID);
			if (Input.GetKeyUp(KeyCode.LeftControl)) this.photonView.RPC("crouch", RpcTarget.AllBuffered, 18, 8, this.photonView.ViewID);
			if (Input.GetKeyDown(KeyCode.LeftShift)) speed = 2.5f;
			if (Input.GetKeyUp(KeyCode.LeftShift)) speed = 5;

			//CODE I COPIED
			// Calculate how fast we should be moving
			if (riding == false)
			{
				Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
				targetVelocity = transform.TransformDirection(targetVelocity);
				targetVelocity *= speed;

				// Apply a force that attempts to reach our target velocity
				Vector3 velocity = rb.velocity;
				Vector3 velocityChange = (targetVelocity - velocity);
				velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
				velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
				velocityChange.y = 0;

				rb.AddForce(velocityChange, ForceMode.VelocityChange);
			}
		}

		//riding
		if (riding)
		{
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			transform.position = vehicle.transform.position;
		}
	}

	public void jump()
	{
		if (grounded)
		{
			bool canJump = true;
			if (cam.GetComponent<holderSpawner>().holding && jumpHit.collider.gameObject == cam.GetComponent<holderSpawner>().obj) canJump = false;
			if (canJump)
			{
				rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
				anim.SetBool("jump", true);
				jumpTimer = true;
			}
		}
	}

	[PunRPC]
	public void crouch(int height, int center, int viewID)
	{
		GameObject obj = PhotonView.Find(viewID).gameObject;
		obj.GetComponent<CapsuleCollider>().height = height;
		obj.GetComponent<CapsuleCollider>().center = new Vector3(0, center, 0);
	}

	private void FixedUpdate()
	{
		if (jumpTimer)
		{
			start += 0.1f;
			if (start >= 2.5f)
			{
				jumpTimer = false;
				anim.SetBool("jump", false);
				start = 0;
			}
		}
	}

	public void vehicleEnter()
	{
		if (riding == false)
		{
			if (vehicle != null && vehicle.GetComponent<vehicle>().currentPlayers < vehicle.GetComponent<vehicle>().maxPlayers)
			{
				vehicle.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
				riding = true; 
				
				cam.SetActive(false);
				cam2.SetActive(true);

				cam.GetComponent<holderSpawner>().enabled = false;
				GetComponent<Collider>().enabled = false;
				transform.position = vehicle.transform.position;
				foreach (Transform child in vehicle.GetComponentInChildren<Transform>())
				{
					child.gameObject.layer = 2;
				}

				if (vehicle.GetComponent<vehicle>().currentPlayers == 0)
				{
					this.photonView.RPC("hideBody", RpcTarget.AllBuffered, body.GetComponent<PhotonView>().ViewID, false);
					vehicle.GetComponent<PhotonView>().TransferOwnership(GetComponent<PhotonView>().Owner);

					if (vehicle.GetComponent<plane>() != null)
					{
						vehicle.GetComponent<plane>().enabled = true;
						vehicle.GetComponent<Rigidbody>().freezeRotation = true;
					}
					else if (vehicle.GetComponent<car>() != null) vehicle.GetComponent<car>().enabled = true;
				}

				this.photonView.RPC("vehicleAddPlayer", RpcTarget.AllBuffered, vehicle.GetComponent<PhotonView>().ViewID, 1);
			}
		}
		else
		{
			if (vehicle != null)
			{
				riding = false;
				cam.GetComponent<holderSpawner>().enabled = true;
				cam.SetActive(true);
				cam2.SetActive(false);
				GetComponent<Collider>().enabled = true;
				foreach (Transform child in vehicle.GetComponentInChildren<Transform>())
				{
					child.gameObject.layer = 10;
				}

				if (vehicle.GetComponent<PhotonView>().IsMine)
				{
					this.photonView.RPC("hideBody", RpcTarget.AllBuffered, body.GetComponent<PhotonView>().ViewID, true);
					if (vehicle.GetComponent<plane>() != null)
					{
						vehicle.GetComponent<plane>().enabled = false;
						vehicle.GetComponent<Rigidbody>().freezeRotation = false;
					}
					else if (vehicle.GetComponent<car>() != null) vehicle.GetComponent<car>().enabled = false;
				}
				this.photonView.RPC("vehicleAddPlayer", RpcTarget.AllBuffered, vehicle.GetComponent<PhotonView>().ViewID, -1);
			}
		}
	}

	[PunRPC]
	public void hideBody(int viewID, bool value)
	{
		PhotonView objview = PhotonView.Find(viewID);
		GameObject obj = objview.gameObject;
		Debug.Log(obj.name);
		obj.SetActive(value);
	}

	[PunRPC]
	public void vehicleAddPlayer(int viewID, int value)
	{
		Debug.Log(viewID + "/" + value);
		PhotonView objview = PhotonView.Find(viewID);
		GameObject obj = objview.gameObject;
		Debug.Log(obj.name);
		obj.GetComponent<vehicle>().currentPlayers += value;
	}
}