using System;
using UnityEngine;

public class AnimateTransform : MonoBehaviour
{
    [Serializable]
    public struct Vector3Oscillation
    {
        public Vector3 Center;
        public Vector3 Amplitude;
        public Vector3 Frequency;
        public Vector3 Offset;
    }

    public Vector3Oscillation Position = new Vector3Oscillation
    {
        Amplitude = Vector3.one * 0.1f,
        Frequency = Vector3.one * 0.1f,
    };
    public Vector3Oscillation Rotation = new Vector3Oscillation
    {
        Amplitude = Vector3.one * 45f,
        Frequency = Vector3.one * 0.1f,
    };
    public Vector3Oscillation Scale = new Vector3Oscillation
    {
        Center = Vector3.one,
        Amplitude = Vector3.one * 0.25f,
        Frequency = Vector3.one * 0.1f,
    };

    void Start()
    {
        Position.Offset = UnityEngine.Random.insideUnitSphere * 100f;
        Rotation.Offset = UnityEngine.Random.insideUnitSphere * 100f;
        Scale.Offset = UnityEngine.Random.insideUnitSphere * 100f;
    }

    void Update()
    {
        transform.localPosition = Oscillate(transform.localPosition, Position);
        transform.localEulerAngles = Oscillate(transform.localEulerAngles, Rotation);
        transform.localScale = Oscillate(transform.localScale, Scale);
    }

    static Vector3 Oscillate(Vector3 vector, Vector3Oscillation settings) => new Vector3(
        settings.Amplitude.x == 0f || settings.Frequency.x == 0f ? vector.x :
        Mathf.Sin(Time.time * settings.Frequency.x + settings.Offset.x) * settings.Amplitude.x + settings.Center.x,
        settings.Amplitude.y == 0f || settings.Frequency.y == 0f ? vector.y :
        Mathf.Sin(Time.time * settings.Frequency.y + settings.Offset.y) * settings.Amplitude.y + settings.Center.y,
        settings.Amplitude.z == 0f || settings.Frequency.z == 0f ? vector.z :
        Mathf.Sin(Time.time * settings.Frequency.z + settings.Offset.z) * settings.Amplitude.z + settings.Center.z);
}
