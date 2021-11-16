using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ComplexSlow : MonoBehaviour
{
    protected ComplexTimeManipulation complexEntity;
    protected virtual void Awake()
    {
        complexEntity = transform.GetComponent<ComplexTimeManipulation>();
    }
    public abstract void Slow(float slowTime, float slowFactor);
    public abstract float[] GetData();
}
