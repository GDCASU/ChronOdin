using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonovanLevelMakeObjectDisappear : MonoBehaviour
{
    [SerializeField] private GameObject objectToMakeDisappear;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            objectToMakeDisappear.SetActive(false);
        }
    }
}
