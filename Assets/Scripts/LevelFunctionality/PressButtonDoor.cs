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
    public int _numOfButtonsRequired;
    public bool isMoving;

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
    }
    public void Increment()
    {
        if (!isMoving)
        {
            _numOfButtonsRequired++;
            UpdateIndicators();
            if (_numOfButtonsRequired == numOfButtonsRequired)
            {
                StartCoroutine(MoveDoor());
            }
        }
    }

    public void Decrement()
    {
        if (!isMoving)
        {
            _numOfButtonsRequired--;
            UpdateIndicators();
            if (_numOfButtonsRequired != numOfButtonsRequired)
            {
                StartCoroutine(MoveDoor());
            }
        }
    }

    IEnumerator MoveDoor()
    {
        isMoving = true;
        GetComponent<FMODPlay3DSoundEffect>().PlaySoundEffect();
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
        isMoving = false;
    }

    private void StartIndicators()
    {
        float indicatorObjectWidth = 1f / (float) numOfButtonsRequired;
        for (int i = 0; i < numOfButtonsRequired; i++) 
        {
            GameObject instantiatedObject = Instantiate(_indicatorObject, Vector2.zero, Quaternion.identity);

            instantiatedObject.transform.SetParent(transform);
            instantiatedObject.transform.localScale = Vector3.one * .75f;
            instantiatedObject.transform.localPosition = new Vector3(0, 2f, (i - (numOfButtonsRequired - 1f) / 2f) * indicatorObjectWidth);
            instantiatedObject.transform.localEulerAngles = new Vector3(0,-90,0);

            instantiatedObject.transform.position += transform.right;
            instantiatedObject.transform.parent = null;

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
