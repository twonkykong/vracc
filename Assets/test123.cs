using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class test123 : MonoBehaviour
{
    public string path;
    public string[] files;
    public RawImage image;

    void Update()
    {
        if (path != null) files = Directory.GetFiles(path);
        GetImage();
    }    

    void GetImage()
    {
        OpenImage();
    }

    void OpenImage()
    {
        WWW www = new WWW("file:///" + "Assets");
        image.texture = www.texture;
    }
}
