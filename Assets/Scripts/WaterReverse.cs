using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterReverse : ObjectReverse
{
    [SerializeField]
    [Tooltip("The upward force objects in a reversed waterfall shall experience")]
    private float reverseWaterForce;

    private bool isReversing = false;

    protected override void Start() { }
    protected override void FixedUpdate() { }
    protected override void Record() { }

    public override IEnumerator Reverse(float reverseTime)
    {
        isReversing = true;
        yield return new WaitForSeconds(reverseTime);
        isReversing = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (isReversing)
        {
            //if(GetComponent<TestMoveThree>()) GetComponent<TestMoveThree>().useGravity = false;
            other.attachedRigidbody.AddForce(0, reverseWaterForce, 0);
        }
        //else if (GetComponent<TestMoveThree>()) GetComponent<TestMoveThree>().useGravity = true;
    }
}
