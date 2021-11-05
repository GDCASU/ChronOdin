using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{

    private float timeScale; 

    public float rotateSpeed;
    public GameObject gameObject;
    
    // Start is called before the first frame update
    void Start()
    {
        UpdateTime();
        MasterTime.singleton.updateTimeScaleEvent += UpdateTime;
    }

    // Update is called once per frame
    void Update()
    {
        rotateSpeed = rotateSpeed * timeScale * Time.fixedDeltaTime;

        gameObject.transform.Rotate(0, rotateSpeed, 0, Space.Self);
    }

    private void UpdateTime() => timeScale = MasterTime.singleton.timeScale;
}
