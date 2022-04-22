using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityUnlock : MonoBehaviour
{
    public TimeEffect abilityToUnlock;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) UnlockAbility();
    }

    public void UnlockAbility()
    {
        PlayerController.singleton.GetComponent<AbilityManager>()?.SetAbilityStatus(abilityToUnlock, true);
        Destroy(gameObject);
    }
}
