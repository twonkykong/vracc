using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class prop : MonoBehaviour
{
    public Mesh mesh;
    public GameObject[] pieces;
    public Material mat;
    public AudioClip audio;

    public void Break()
    {
        if (mesh != null) GetComponent<MeshFilter>().mesh = mesh;
        if (mat != null) GetComponent<Renderer>().material = mat;
        if (pieces.Length != 0)
        {
            foreach (GameObject piece in pieces)
            {
                GameObject g = Instantiate(piece, transform.position, transform.rotation);
                g.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity / 2;
            }
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (audio != null)
        {
            if (collision.gameObject.tag != "Player")
            {
                GetComponent<AudioSource>().Stop();
                GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1);
                GetComponent<AudioSource>().clip = audio;
                GetComponent<AudioSource>().Play();
            }
        }
    }
}
