using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hand : MonoBehaviour
{
    public Animation anim;
    bool animate;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) animate = true;
        if (Input.GetKeyUp(KeyCode.B)) animate = false;

        if (Input.GetKeyDown(KeyCode.Alpha1) && animate == true) play("hello");
    }

    void play(string anim_name)
    {
        anim.Play(anim_name);
    }
}
