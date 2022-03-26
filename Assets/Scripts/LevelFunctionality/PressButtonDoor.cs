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

    public int numOfButtonsRequired;
    private int _numOfButtonsRequired = 0;

    [SerializeField] private Material _litMaterial;
    [SerializeField] private Material _unlitMaterial;
    public GameObject _indicatorObject;
    private List<GameObject> _indicatorObjects = new List<GameObject>();
    private const float _offsetWidth = 10f;

    protected void Start()
    {
        UpdateWithGlobalTimescale(MasterTime.singleton.timeScale);
        originalPosition = transform.position;
        originalRotation = transform.rotation.eulerAngles;
        _numOfButtonsRequired = 0;
        if (!swingsOpen) moveToVector += originalPosition;

        StartIndicators();
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

    private void StartIndicators()
    {
        float indicatorObjectWidth = 1f / (float) numOfButtonsRequired;
        for (int i = 0; i < numOfButtonsRequired; i++) 
        {
            GameObject instantiatedObject = Instantiate(_indicatorObject, Vector2.zero, Quaternion.identity);

            instantiatedObject.transform.SetParent(transform);
            instantiatedObject.transform.localScale = new Vector3(1.2f, 0.3f, indicatorObjectWidth);
            instantiatedObject.transform.localPosition = new Vector3(0, 0.5f, (i - (numOfButtonsRequired - 1f) / 2f) * indicatorObjectWidth);
            instantiatedObject.transform.localEulerAngles = new Vector3(0,0,0);

            _indicatorObjects.Add(instantiatedObject);
        }
    }

    private void UpdateIndicators() 
    {
        for (int i = 0; i < _indicatorObjects.Count; i++) 
        {
            MeshRenderer indicatorObjectMeshRenderer = _indicatorObjects[i].GetComponent<MeshRenderer>();
            if (i > numOfButtonsRequired - _numOfButtonsRequired - 1) indicatorObjectMeshRenderer.material = _litMaterial;
            else indicatorObjectMeshRenderer.material = _unlitMaterial;
        }
    }
}
