using UnityEngine;

public class Controller : MonoBehaviour
{
    public float Speed = 5f;

    void Update()
    {
        /*
        - add quad with streaming camera
        - stream with additive shader
        - do not render the quad on the main camera
        */
        var motion = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        transform.localPosition += motion * Time.deltaTime * Speed;
    }
}
