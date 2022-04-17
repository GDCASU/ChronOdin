using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class VisualHostHandler : MonoBehaviour
{
    private VisualEffect vfx;
    private bool vfxActive = false;

    private float delayedCheckTime = 0.5f;
    private float elapsedTime = 0f;

    private void Awake()
    {
        vfx = GetComponentInChildren<VisualEffect>();
    }

    private void Update()
    {
        if (vfxActive)
        {
            if (elapsedTime > delayedCheckTime)
            {
                if (vfx.aliveParticleCount == 0)
                {
                    Destroy(gameObject);
                }
            }
            else
                elapsedTime += Time.deltaTime;
        }
    }

    public VisualEffect GetVFX() => vfx;

    public void PlayVFX()
    {
        vfx.Play();
        vfxActive = true;
    }
}
