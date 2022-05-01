using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class technology : MonoBehaviour
{
    public Animator anim;
    public string componentName;
    public GameObject generator;
    public Material mat;

    public bool work;

    private void Update()
    {
        if (work)
        {
            GetComponent<LineRenderer>().SetPositions(new Vector3[2] { transform.position, generator.transform.position });
            if (Vector3.Distance(transform.position, generator.transform.position) > 20) power(false);
        }
    }

    public void power(bool value)
    {
        work = value;
        if (componentName != "") (GetComponent(componentName) as MonoBehaviour).enabled = value;
        if (anim != null) anim.enabled = value;
        if (value)
        {
            gameObject.AddComponent<LineRenderer>();
            GetComponent<LineRenderer>().startWidth = 0.15f;
            GetComponent<LineRenderer>().material = mat;
        }
        else
        {
            Destroy(GetComponent<LineRenderer>());
            foreach (GameObject obj in generator.GetComponent<generator>().connectings)
            {
                if (obj == gameObject) generator.GetComponent<generator>().connectings.Remove(obj);
            }
        }
    }
}
