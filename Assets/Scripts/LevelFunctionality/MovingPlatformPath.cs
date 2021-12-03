using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformPath : SimpleTimeManipulation
{
    public GameObject platform;
    public float speed;
    public float delay;
    public bool loop;
    public Vector3[] points;
    private float speedMultiplier = 1f;

    void Start()
    {
        timescale = MasterTime.singleton.timescale;
        if (points.Length > 1)
        {
            if (loop) StartCoroutine(LoopPlatform());
            else StartCoroutine(MovePlatformForward());
        }
    }

    IEnumerator MovePlatformForward()
    {
        platform.transform.position = transform.position + points[0];
        for (int i = 0; i < points.Length - 1; i++)
        {
            float step = 0;
            while (step < 1)
            {
                step += speed * speedMultiplier * timescale * Time.fixedDeltaTime * 1f / Vector3.Distance(points[i], points[i + 1]);
                platform.transform.position = transform.position + Vector3.Lerp(points[i], points[i + 1], step);
                yield return new WaitForFixedUpdate();
            }
            if (delay > 0) yield return new WaitForSeconds(delay);
        }
        StartCoroutine(MovePlatformBackWards());
    }
    IEnumerator MovePlatformBackWards()
    {
        platform.transform.position = transform.position + points[0];
        for (int i = points.Length - 1; i > 1 - 1; i--)
        {
            float step = 0;
            while (step < 1)
            {
                step += speed * speedMultiplier * timescale *Time.fixedDeltaTime * 1f / Vector3.Distance(points[i], points[i - 1]);
                platform.transform.position = transform.position + Vector3.Lerp(points[i], points[i - 1], step);
                yield return new WaitForFixedUpdate();

            }
            if (delay > 0) yield return new WaitForSeconds(delay);
        }
        StartCoroutine(MovePlatformForward());
    }
    IEnumerator LoopPlatform()
    {
        int i = 0;
        while (enabled)
        {
            float step = 0;
            while (step < 1)
            {
                step += speed * speedMultiplier * timescale * Time.fixedDeltaTime * 1f / Vector3.Distance(points[i], points[(i + 1 >= points.Length) ? 0 : i + 1]);
                platform.transform.position = transform.position + Vector3.Lerp(points[i], points[(i + 1 >= points.Length) ? 0 : i + 1], step);
                yield return new WaitForFixedUpdate();
            }
            i++;
            i = i % (points.Length);
            if (delay > 0) yield return new WaitForSeconds(delay);
        }
    }
}
