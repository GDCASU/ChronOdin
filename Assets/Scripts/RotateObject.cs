using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{

    private float timeScale;

    [SerializeField]
    private float rotateSpeed = 2;
    //public GameObject pillar;

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
