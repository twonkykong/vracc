using UnityEngine;
using System;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class bodyMobile : MonoBehaviourPunCallbacks
{

	public float speed = 5.0f;
	public float maxVelocityChange = 10.0f;
	public Rigidbody rb;

	Vector3 FirstPoint;
	Vector3 SecondPoint;
	float xAngle;
	float yAngle;
	float xAngleTemp;
	float yAngleTemp;

	public GameObject cam1, cam2, cam2holder, head, headSkin, gameManager, joystick, joystickParent, vehicle, vehicleButton, brakebtn, body;
	public GameObject[] ridingFalse, ridingTrue, vroff;
	public float jumpForce, hor, vert, rotx, roty, rotz, rotw, magn, sensitivity;
	public bool riding;
	bool jumpTimer, grounded, crouching, runing, brake, vr;
	public Text weaponsText;
	public Vector3 lookat, pos, acc;
	public Animator anim;

	int touchid;
	float start;

	private void Start()
	{
		if (!GetComponent<PhotonView>().IsMine) return;

		sensitivity = PlayerPrefs.GetInt("sens") / 100;
		cam1.GetComponent<Camera>().fieldOfView = PlayerPrefs.GetInt("fov");

		rb = GetComponent<Rigidbody>();

		//copied
		xAngle = 0;
		yAngle = 0;
		transform.rotation = Quaternion.Euler(transform.eulerAngles.x, xAngle, 0);
		head.transform.rotation = Quaternion.Euler(-yAngle, head.transform.eulerAngles.y, 0);

		PlayerPrefs.SetInt("vr", 0);

		if (PlayerPrefs.GetInt("vr") == 1) vr = true;
		else vr = false;
	}

	private void Update()
	{
		if (!GetComponent<PhotonView>().IsMine) return;

		if (GetComponent<Player>().cursorLock == false)
		{
			//animations
			if (hor != 0 || vert != 0) anim.SetBool("walk", true);
			else anim.SetBool("walk", false);

			//head rotation
			if (Input.touchCount > 0)
			{
				if (Input.touchCount > 1)
				{
					foreach (Touch touch in Input.touches) if (touch.position.x > Screen.width / 2) touchid = touch.fingerId;
				}
				else touchid = 0;

				if (Input.GetTouch(touchid).position.x > Screen.width / 2)
				{
					if (Input.GetTouch(touchid).phase == TouchPhase.Began)
					{
						FirstPoint = Input.GetTouch(touchid).position;
						xAngleTemp = xAngle;
						yAngleTemp = yAngle;
					}
					if (Input.GetTouch(touchid).phase == TouchPhase.Moved)
					{
						SecondPoint = Input.GetTouch(touchid).position;
						xAngle = xAngleTemp + (SecondPoint.x - FirstPoint.x) * 180 * sensitivity / Screen.width;
						yAngle = yAngleTemp + (SecondPoint.y - FirstPoint.y) * 90 * sensitivity / Screen.height;

						if (yAngle >= 89.9f) yAngle = 89.9f;
						if (yAngle <= -89.9f) yAngle = -89.9f;
						if (xAngle >= 360 || xAngle <= -360) xAngle = 0;

						headSkin.transform.rotation = head.transform.rotation = Quaternion.Euler(-yAngle, head.transform.eulerAngles.y, 0.0f);
						transform.rotation = Quaternion.Euler(transform.eulerAngles.x, xAngle, 0.0f);
					}
				}
			}

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
			if (Physics.Raycast(transform.position + transform.up, -transform.up, 1.3f)) grounded = true;
			else grounded = false;

			//CODE I kinda COPIED
			//inputs
			hor = joystick.transform.localPosition.x / 128;
			vert = joystick.transform.localPosition.y / 128;

			Debug.Log(joystick.transform.localPosition);

			// Calculate how fast we should be moving
			Vector3 targetVelocity = new Vector3(hor, 0, vert);
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

		RaycastHit hit;
		if (Physics.Raycast(cam1.transform.position, cam1.transform.forward, out hit, 4f))
		{
			//vehicles
			if (hit.collider.tag == "vehicle" && riding == false)
			{
				vehicle = hit.collider.transform.parent.gameObject;
				vehicleButton.SetActive(true);
			}
		}
		else
		{
			if (riding == false) vehicle = null;
			vehicleButton.SetActive(false);
		}

		//riding
		if (riding)
		{
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			transform.position = vehicle.transform.position;

			foreach (GameObject obj in ridingTrue) obj.SetActive(true);
			foreach (GameObject obj in ridingFalse) obj.SetActive(false);

			if (vehicle.GetComponent<car>() != null)
			{
				brakebtn.SetActive(true);
				vehicle.GetComponent<car>().motortorque = vert * vehicle.GetComponent<car>().speed;
				vehicle.GetComponent<car>().steerangle = hor * vehicle.GetComponent<car>().steer;

			}
			else if (vehicle.GetComponent<plane>() != null)
			{
				vehicle.GetComponent<plane>().rotate("x", 2 * vert);
				vehicle.GetComponent<plane>().rotate("y", 2 * -hor);
				brakebtn.SetActive(false);
			}
		}
		else
		{
			foreach (GameObject obj in ridingTrue) obj.SetActive(false);
			foreach (GameObject obj in ridingFalse) obj.SetActive(true);

			brakebtn.SetActive(false);
		}
	}

	public void jump()
	{
		if (grounded)
		{
			rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
			anim.SetBool("jump", true);
			jumpTimer = true;
		}
	}

	public void run()
	{
		if (runing == false)
		{
			runing = true;
			speed = 10.0f;
		}
		else
		{
			runing = false;
			speed = 5.0f;
		}
	}

	[PunRPC]
	public void crouch(int viewID)
	{
		
		if (crouching == false)
		{
			this.photonView.RPC("crouchRPC", RpcTarget.AllBuffered, 9, 12, this.photonView.ViewID);
			crouching = true;
		}
		else
		{
			if (Physics.Raycast(transform.position, transform.up, 1.3f))
			{
				transform.position += transform.up;
				this.photonView.RPC("crouchRPC", RpcTarget.AllBuffered, 18, 8, this.photonView.ViewID);
				crouching = false;
			}
		}
	}

	[PunRPC]
	public void crouchRPC(int height, int center, int viewID)
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

				cam1.SetActive(false);
				cam2.SetActive(true);

				cam1.GetComponent<holderSpawner>().enabled = false;
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
				cam1.GetComponent<holderSpawner>().enabled = true;
				cam1.SetActive(true);
				cam2.SetActive(false);
				GetComponent<Collider>().enabled = true;
				foreach (Transform child in vehicle.GetComponentInChildren<Transform>())
				{
					child.gameObject.layer = 10;
				}
				this.photonView.RPC("vehicleAddPlayer", RpcTarget.AllBuffered, vehicle.GetComponent<PhotonView>().ViewID, -1);

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
			}
		}
	}

	public void Brake()
	{
		if (brake == false)
		{
			vehicle.GetComponentInChildren<WheelCollider>().motorTorque = 0;
			vehicle.GetComponent<car>().braketorque = vehicle.GetComponent<car>().brake;
			brake = true;
		}
		else
		{
			vehicle.GetComponent<car>().braketorque = 0;
			brake = false;
		}
	}

	[PunRPC]
	public void hideBody(int viewID, bool value)
	{
		GameObject obj = PhotonView.Find(viewID).gameObject;
		obj.SetActive(value);
	}

	[PunRPC]
	public void vehicleAddPlayer(int viewID, int value)
	{
		GameObject obj = PhotonView.Find(viewID).gameObject;
		obj.GetComponent<vehicle>().currentPlayers += value;
	}
}
