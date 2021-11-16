using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ComplexFreeze : MonoBehaviour
{
    protected ComplexTimeManipulation complexEntity;
    protected virtual void Awake()
    {
        complexEntity = transform.GetComponent<ComplexTimeManipulation>();
    }
    public abstract void Freeze(float freezeTime);
    public abstract float[] GetData();
}
