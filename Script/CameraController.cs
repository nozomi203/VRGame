using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float rotateSpeed;

    private int rotateFactor = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        RotateCamera();
    }

    private void RotateCamera()
    {
        if (Input.GetKey(KeyCode.A))
        {
            if (!Input.GetKey(KeyCode.D))
            {
                rotateFactor = 1;
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rotateFactor = -1;
        }
        else
        {
            rotateFactor = 0;
        }
        transform.forward = Quaternion.AngleAxis(rotateSpeed * Time.deltaTime * rotateFactor, Vector3.up) * transform.forward;
    }
}
