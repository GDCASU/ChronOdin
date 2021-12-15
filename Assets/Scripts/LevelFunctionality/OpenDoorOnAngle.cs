using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorOnAngle : MonoBehaviour
{
    public float angleRequiredToOpen;
    public float angleRange;
    private float yRotation;
    private float addition;
    private float subtraction;
    public GameObject[] linkedObjects;
    public bool inRange;

    private void Update()
    {
        yRotation = transform.rotation.eulerAngles.y;
        addition = angleRequiredToOpen + angleRange;
        subtraction = angleRequiredToOpen - angleRange;
        if (subtraction < 0)
        {
            float overflow = 360 + (subtraction);
            if ((yRotation > overflow && yRotation < 0) || (yRotation >= 0 && yRotation < addition))
            {
                if (!inRange)
                {
                    foreach (GameObject pressureObject in linkedObjects) pressureObject.GetComponent<LinkedToPressurePlate>().Activate();
                    inRange = true;
                }
            }
            else if (inRange)
            {
                foreach (GameObject pressureObject in linkedObjects) pressureObject.GetComponent<LinkedToPressurePlate>().Deactivate();
                inRange = false;
            }
        }
        else if (addition > 360)
        {
            float overflow = 360 - (addition);
            if ((yRotation > subtraction && yRotation < 360) || (yRotation >= 0 && yRotation < overflow))
            {
                if (!inRange)
                {
                    foreach (GameObject pressureObject in linkedObjects) pressureObject.GetComponent<LinkedToPressurePlate>().Activate();
                    inRange = true;
                }
            }
            else if (inRange)
            {
                foreach (GameObject pressureObject in linkedObjects) pressureObject.GetComponent<LinkedToPressurePlate>().Deactivate();
                inRange = false;
            }
        }
        else
        {
            if (yRotation > subtraction && yRotation < addition)
            {
                if (!inRange)
                {
                    foreach (GameObject pressureObject in linkedObjects) pressureObject.GetComponent<LinkedToPressurePlate>().Activate();
                    inRange = true;
                }
            }
            else if (inRange)
            {
                foreach (GameObject pressureObject in linkedObjects) pressureObject.GetComponent<LinkedToPressurePlate>().Deactivate();
                inRange = false;
            }
        }
    }
}
