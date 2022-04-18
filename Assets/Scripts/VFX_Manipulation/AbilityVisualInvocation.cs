using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class AbilityVisualInvocation : MonoBehaviour
{
    [SerializeField]
    private GameObject singleAbilityHost;

    [SerializeField]
    private GameObject environmentAbilityHost;

    public static AbilityVisualInvocation singleton;

    private static readonly ExposedProperty lightningLength = "lightningLength";

    private void Awake()
    {
        if (singleton == null)
            singleton = this;
        else
            Destroy(gameObject);
    }

    public void PlaySingleVFX()
    {
        RaycastHit rayHit = PlayerInteractions.singleton.rayHit;

        Transform newHost = Instantiate(singleAbilityHost, transform.position, Quaternion.identity).transform;
        VisualHostHandler handler = newHost.GetComponent<VisualHostHandler>();

        float distance = (rayHit.point - newHost.position).magnitude;
        newHost.LookAt(rayHit.point);

        handler.GetVFX().SetFloat(lightningLength, distance * 10);
        handler.PlayVFX();
    }

    public void PlayerEnvironmentVFX()
    {
        Transform newHost = Instantiate(environmentAbilityHost, transform.position, Quaternion.identity).transform;
        VisualHostHandler handler = newHost.GetComponent<VisualHostHandler>();

        handler.PlayVFX();
    }
}
