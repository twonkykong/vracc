using Photon.Voice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bigfan : MonoBehaviour
{
    public GameObject propeller;
    JointMotor motor;

    private void Start()
    {
        motor.targetVelocity = 200;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Rigidbody>() != null)
        {
            other.GetComponent<Rigidbody>().AddForce(transform.forward * 2f, ForceMode.Impulse);
            motor.force = 50;
            propeller.GetComponent<HingeJoint>().motor = motor;
        }
        else
        {
            motor.force = 0;
            propeller.GetComponent<HingeJoint>().motor = motor;
        }
    }
}
