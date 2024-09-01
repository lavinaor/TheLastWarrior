using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lockAtCamera : MonoBehaviour
{
    private Transform camTransform;

    void Start()
    {
        camTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + camTransform.rotation * Vector3.forward, camTransform.rotation * Vector3.up);
    }
}
