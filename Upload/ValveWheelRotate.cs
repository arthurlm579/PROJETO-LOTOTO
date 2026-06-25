using UnityEngine;

public class ValveWheelRotate : MonoBehaviour
{
    public float rotationSpeed = 80f;
    public bool rotateWithInput = true;

    void Update()
    {
        if (rotateWithInput)
        {
            if (Input.GetKey(KeyCode.E))
            {
                transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
            }

            if (Input.GetKey(KeyCode.Q))
            {
                transform.Rotate(Vector3.down * rotationSpeed * Time.deltaTime, Space.Self);
            }
        }
        else
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}