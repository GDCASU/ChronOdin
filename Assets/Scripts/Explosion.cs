/*
 * Simulates an explosion from an attached object.
 * Press 'Space' to initiate the explosion.
 * 
 * Author: Cristion Dominguez
 * Date: 22 July 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Radius objects shall be affected in.")]
    private float radius = 5f;

    [SerializeField]
    [Tooltip("Force applied to affected objects.")]
    private float power = 600f;

    private Vector3 position;
    private Collider[] colliders;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Explode();
        }
    }

    public void Explode()
    {
        position = transform.position;
        colliders = Physics.OverlapSphere(position, radius);

        foreach(Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(power, position, radius, 3.0f);
            }
        }
    }
}
