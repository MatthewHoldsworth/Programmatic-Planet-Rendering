using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    float RotateSpeed=1f;
    [SerializeField]
    float MoveSpeed=1f;

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(-Input.GetAxis("Vertical") *RotateSpeed * Time.deltaTime, Input.GetAxis("Horizontal") * RotateSpeed * Time.deltaTime, 0f);
    }
}
