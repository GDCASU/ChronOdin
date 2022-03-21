using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Added generic type (T) for the Interact function.
public interface InteractiveObject<T>
{
    public T Interact();
}
