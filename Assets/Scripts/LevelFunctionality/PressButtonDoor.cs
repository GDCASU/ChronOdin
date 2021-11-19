using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressButtonDoor :MonoBehaviour, LinkedToPressButton
{
    public Vector3 moveToVector;
    public bool swingsOpen;
    public float speed;
    private Vector3 originalPosition;
    private Vector3 originalRotation;
    private bool open;
    private float timeScale;

    private void Start()
    {
        UpdateTime();
        MasterTime.singleton.updateTimeScaleEvent += UpdateTime;
        originalPosition = transform.position;
        originalRotation = transform.rotation.eulerAngles;
        if (!swingsOpen) moveToVector += originalPosition;
    }
    public void Activate()=> StartCoroutine(MoveDoor());
    public void Deactivate() => StartCoroutine(MoveDoor());
    IEnumerator MoveDoor()
    {
        Vector3 endPosition = open?(swingsOpen ? originalRotation: originalPosition): moveToVector;
        Vector3 startingPosition = swingsOpen? transform.rotation.eulerAngles : transform.position;
        Vector3 lerpVector;
        float step = 0;
        while (step < 1)
        {
            step += speed * timeScale * Time.fixedDeltaTime;
            lerpVector = Vector3.Lerp(startingPosition, endPosition, step);
            if (swingsOpen) transform.rotation = Quaternion.Euler(lerpVector);
            else transform.position = lerpVector;
            yield return new WaitForFixedUpdate();
        }
        open = !open;
    }
    private void UpdateTime() => timeScale = MasterTime.singleton.timeScale;
}
