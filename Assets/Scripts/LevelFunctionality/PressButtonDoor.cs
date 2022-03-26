using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressButtonDoor: SimpleTimeManipulation, LinkedToPressButton
{
    public Vector3 moveToVector;
    public bool swingsOpen;
    public float speed;
    private Vector3 originalPosition;
    private Vector3 originalRotation;
    private bool open;

    public int numOfButtonsRequired = 1;
    private int _numOfButtonsRequired = 1;

    [SerializeField] private Material _litMaterial;
    [SerializeField] private Material _unlitMaterial;
    [SerializeField] private List<GameObject> _indicatorObjects;

    protected void Start()
    {
        UpdateWithGlobalTimescale(MasterTime.singleton.timeScale);
        originalPosition = transform.position;
        originalRotation = transform.rotation.eulerAngles;
        _numOfButtonsRequired = 0;
        if (!swingsOpen) moveToVector += originalPosition;
        UpdateIndicators();
    }
    public void Increment()
    {
        _numOfButtonsRequired++;
        if (_numOfButtonsRequired == numOfButtonsRequired)
        {
            StartCoroutine(MoveDoor());
        }
        UpdateIndicators();
    }
    public void Decrement()
    {
        if (_numOfButtonsRequired == numOfButtonsRequired)
        {
            StartCoroutine(MoveDoor());
        }
        _numOfButtonsRequired--;
        UpdateIndicators();
    }

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

    private void UpdateIndicators() 
    {
        for (int i = 0; i < _indicatorObjects.Count; i++) 
        {
            MeshRenderer indicatorObjectMeshRenderer = _indicatorObjects[i].GetComponent<MeshRenderer>();
            if (i < _numOfButtonsRequired) indicatorObjectMeshRenderer.material = _litMaterial;
            else indicatorObjectMeshRenderer.material = _unlitMaterial;
        }
    }
}
