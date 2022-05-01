using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;

public class car : MonoBehaviour
{
    public WheelCollider[] wheels;
    public float speed, steer, brake, motortorque, braketorque, steerangle;

    bool pc;

    private void Start()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) pc = false;
        else pc = true;
    }

    private void Update()
    {
        if (pc == true)
        {
            motortorque = Input.GetAxis("Vertical") * speed;
            braketorque = System.Convert.ToSingle(Input.GetKey(KeyCode.Space)) * brake;
            Debug.Log(braketorque);
            steerangle = Input.GetAxis("Horizontal") * steer;
        }

        foreach (WheelCollider wheel in wheels)
        {
            if (wheel.name == "l1")
            {
                wheel.steerAngle = steerangle;
                wheel.transform.localEulerAngles = new Vector3(wheel.transform.localEulerAngles.x, wheel.steerAngle - 90, wheel.transform.localEulerAngles.z);
            }
            else if (wheel.name == "r1")
            {
                wheel.steerAngle = steerangle;
                wheel.transform.localEulerAngles = new Vector3(wheel.transform.localEulerAngles.x, wheel.steerAngle + 90, wheel.transform.localEulerAngles.z);
            }
            else
            {
                wheel.motorTorque = motortorque;
                //wheel.brakeTorque = braketorque;
            }

            //wheels mesh rotate
            //wheel.transform.localEulerAngles = new Vector3(wheel.transform.localEulerAngles.x, wheel.steerAngle - 90, wheel.transform.localEulerAngles.z);
            if (braketorque != 0) GetComponent<Rigidbody>().velocity /= braketorque;
        }
    }
}
