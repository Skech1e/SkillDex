using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ElementBtnAnimator : MonoBehaviour
{
    public float radius;
    public E_ArrangeType Arrangement;
    public List<Transform> elements;
    public float animSpeed;

    private void Awake()
    {
    }

    private void OnValidate()
    {
        elements = new();
        int count = transform.childCount;
        for (int i = 0; i < count; i++) elements.Add(transform.GetChild(i).transform);
    }

    private float[] ElementAngles()
    {
        float[] angles = new float[elements.Count];
        float angleDelta = Arrangement switch
        {
            E_ArrangeType.Circular => -360f / elements.Count,
            E_ArrangeType.SemiCircularL => -180f / (elements.Count - 1),
            E_ArrangeType.SemiCircularR => 180f / (elements.Count - 1),
            _ => 0f
        };
        float sectionAngle = 0;
        for (int i = 0; i < elements.Count; i++)
        {
            angles[i] = sectionAngle;
            sectionAngle += angleDelta;
        }
        return angles;
        
    }

    private float GetAngleFromPosition(Vector2 pos) => Mathf.Atan2(pos.x, pos.y) * Mathf.Rad2Deg;

    [ContextMenu("Arrange")]
    public void Animate() => StartCoroutine(AnimateArrangement());

    public float time;
    private IEnumerator AnimateArrangement()
    {
        yield return null;
        float startTime = Time.unscaledTime;
        float[] startAngles = new float[elements.Count];
        float[] targetAngles = new float[elements.Count];
        for (int i = 0; i < elements.Count; i++)
        {
            float currentAngle = GetAngleFromPosition(elements[i].localPosition);
            startAngles[i] = currentAngle;
        }
        targetAngles = ElementAngles();
        float timer = 0;
        while (timer < animSpeed)
        {
            timer = Time.unscaledTime - startTime;
            float t = Mathf.Clamp01(timer / animSpeed);
            t = Mathf.SmoothStep(0, 1, t);
            time = t;
            for (int i = 0; i < elements.Count; i++)
            {
                float lerpAngle = Mathf.LerpAngle(startAngles[i], targetAngles[i], t);
                float radians = lerpAngle * Mathf.Deg2Rad;
                Vector2 pos = new Vector2(radius * Mathf.Sin(radians), radius * Mathf.Cos(radians));
                elements[i].localPosition = pos;
            }
            yield return null;
        }
    }
}

public enum E_ArrangeType
{
    Circular,
    SemiCircularL,
    SemiCircularR
}