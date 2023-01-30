using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axis : MonoBehaviour
{
    [SerializeField]
    float rotateSpeed;
    [SerializeField]
    float tilt;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.Rotate(tilt,0f,0f, Space.World);
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(0f,rotateSpeed * Time.deltaTime,0f,Space.Self);
    }
}
