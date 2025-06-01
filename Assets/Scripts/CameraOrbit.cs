using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public float rotationSpeed = 30f;
    public float targetHeight = 2f;
    public Transform target;
    private Vector3 offset;
    private float currentRotationAngle = 0f;

    void Start()
    {

        if (target == null)
        {
            GameObject targetObject = new GameObject("CameraOrbitTarget");
            targetObject.transform.position = Vector3.zero;
            target = targetObject.transform;
        }

 
        offset = transform.position - target.position;
        transform.position = target.position + Vector3.up * targetHeight + offset;
        transform.LookAt(target);
    }

    void LateUpdate()
    {
        currentRotationAngle += rotationSpeed * Time.deltaTime;
        Quaternion rotation = Quaternion.Euler(0f, currentRotationAngle, 0f);
        transform.position = target.position + rotation * offset + Vector3.up * targetHeight;
        transform.LookAt(target);
    }
}

