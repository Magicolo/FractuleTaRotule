using UnityEngine;

public class AnimateProperty : MonoBehaviour
{
    public string Property;
    public float Center;
    public float Amplitude;
    public float Frequency;
    public float Offset;
    public Renderer Renderer;

    void Start() => Offset = Random.value * 100f;
    void Update() => Renderer.material.SetFloat(Property, Mathf.Sin(Time.time * Frequency + Offset) * Amplitude + Center);
}
