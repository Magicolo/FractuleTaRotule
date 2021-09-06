using UnityEngine;

public class Display : MonoBehaviour
{
    public int Index;
    public int Width = 4096;
    public int Height = 4096;
    public int Rate = 60;
    public Renderer[] Renderers;

    void Start()
    {
        var device = WebCamTexture.devices[Index];
        Debug.Log($"Displaying device '{device.name}' at index '{Index}'.");
        var texture = new WebCamTexture(device.name, Width, Height, Rate) { autoFocusPoint = null, };
        foreach (var renderer in Renderers) renderer.material.mainTexture = texture;
        texture.Play();
    }

    void Update() => Cursor.visible = false;
}
