using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityUnlock : MonoBehaviour
{
    public TimeEffect abilityToUnlock;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<AbilityManager>()?.SetAbilityStatus(abilityToUnlock, true);
            Destroy(gameObject);
        }
    }
}
