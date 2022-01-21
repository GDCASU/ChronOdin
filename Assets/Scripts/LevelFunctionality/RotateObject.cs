/*
 * Revision Author: Cristion Dominguez
 * Modification: The script has 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : SimpleTimeManipulation
{
    [SerializeField]
    private float rotationSpeed = 2;
    private float _rotationSpeed;
    public bool stepRotation;
    [HideInInspector] public bool isRotating;
    [Tooltip("Only worry about this if Step Rotation is enabled")]
    public float stepRotationAngle;
    [Tooltip("Only worry about this if Step Rotation is enabled")]
    public float stopTime = 1;
    public float yRotation;
    private float interpolationValue;
    private void Start()
    {
        UpdateTimescale(MasterTime.singleton.timeScale);
        yRotation = transform.rotation.eulerAngles.y;
        isRotating = true;
    }
    void Update()
    {
        _rotationSpeed = rotationSpeed * timeScale * Time.fixedDeltaTime;
        if (!stepRotation)
        {
            transform.Rotate(0, _rotationSpeed, 0, Space.Self);
        }
        else
        {
            if (isRotating)
            {
                if (interpolationValue < 1)
                {
                    float setToAngle = yRotation + Mathf.Lerp(0, 1, interpolationValue) * stepRotationAngle;                    
                    transform.rotation = transform.localRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, setToAngle, transform.rotation.eulerAngles.z);
                    interpolationValue += _rotationSpeed  * timeScale * Time.fixedDeltaTime;
                }
                else
                {
                    isRotating = false;
                    yRotation += stepRotationAngle;
                    yRotation %= 360;
                    transform.rotation = transform.localRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, yRotation, transform.rotation.eulerAngles.z);
                    StartCoroutine(RotationCooldown());
                }
            }
        }
    }
    IEnumerator RotationCooldown()
    {
        float coolDownTime = stopTime / MasterTime.singleton.timeScale;
        float elapsedTime = 0;
        while (elapsedTime < coolDownTime)
        {
            coolDownTime = stopTime / MasterTime.singleton.timeScale;
            elapsedTime += Time.deltaTime * MasterTime.singleton.timeScale;
            yield return null;
        }
        interpolationValue = 0;
        isRotating = true;
    }
    
}