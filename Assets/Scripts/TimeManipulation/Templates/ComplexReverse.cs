/*
 * Parent class for any gameobject that should have their own unique definition of "reverse".
 * 
 * Author: Cristion Dominguez
 * Date: 29 Oct. 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ComplexReverse : MonoBehaviour
{
    public abstract void Reverse(float reverseTime);
}
