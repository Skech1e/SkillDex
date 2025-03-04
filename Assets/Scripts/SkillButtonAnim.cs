using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillButtonAnim : MonoBehaviour
{
    public float radius;
    public bool SkillMenuActive;
    public List<Transform> elements;
    public float skillBtnAnimSpeed, elementAnimSpeed;
    public Vector2 startPosition, targetPosition;
    public float targetScale;

    private void Awake()
    {
    }

    private void Start()
    {
        startPosition = transform.localPosition;
    }

    private void OnValidate()
    {
        elements = new();
        int count = transform.childCount;
        for (int i = 0; i < count; i++) elements.Add(transform.GetChild(i).transform);
    }

    private float[] ElementTargetAngles()
    {
        float[] angles = new float[elements.Count];
        float angleDelta = SkillMenuActive ? (-180f / (elements.Count - 1)) : (-360f / elements.Count);
        float sectionAngle = 0;

        for (int i = 0; i < elements.Count; i++)
        {
            angles[i] = sectionAngle;
            sectionAngle += angleDelta;
        }

        return angles;
    }

    private float GetAngleFromPosition(Vector2 pos)
    {
        Debug.LogWarning(pos + " "+Mathf.Atan2(pos.x, pos.y) * Mathf.Rad2Deg);
        return Mathf.Atan2(pos.x, pos.y) * Mathf.Rad2Deg;
    }
    
    private Vector2 ElementScale(Transform t)
    {
        float angle = GetAngleFromPosition(t.localPosition);
        float difference = Mathf.Abs(angle + 90f);
        float scale = difference switch
        {
            0f => 1.35f,
            45f => 1.2f,
            >45f => 1f,
            _ => 1f
        };
        return new Vector2(scale, scale);
    }

    
    [ContextMenu("Arrange")]
    public void Animate()
    {
        StartCoroutine(AnimateButton());
        StartCoroutine(AnimateElements());
    }

    public float time;

    private IEnumerator AnimateButton()
    {
        yield return null;
        SkillMenuActive = !SkillMenuActive;
        float startTime = Time.unscaledTime;
        Vector2 startPos = transform.localPosition;
        Vector2 targetPos = SkillMenuActive ? targetPosition : startPosition;
        float startScale = transform.localScale.x;
        float finalScale = SkillMenuActive ? targetScale : 1f;

        float timer = 0;
        while (timer < skillBtnAnimSpeed)
        {
            timer = Time.unscaledTime - startTime;
            float t = Mathf.Clamp01(timer / skillBtnAnimSpeed);
            t = Mathf.SmoothStep(0, 1, t);
            float x = Mathf.Lerp(startPos.x, targetPos.x, t);
            float y = Mathf.Lerp(startPos.y, targetPos.y, t);
            transform.localPosition = new Vector2(x, y);
            float scale = Mathf.Lerp(startScale, finalScale, t);
            transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
    }

    private IEnumerator AnimateElements()
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

        targetAngles = ElementTargetAngles();
        float timer = 0;
        while (timer < elementAnimSpeed)
        {
            timer = Time.unscaledTime - startTime;
            float t = Mathf.Clamp01(timer / elementAnimSpeed);
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

    public void ScrollElement(Element element) => StartCoroutine(ElementScroll(element));
    private IEnumerator ElementScroll(Element element)
    {
        yield return null;
        float startTime = Time.unscaledTime;
        float angleDelta = -180f / (elements.Count - 1);
        float angle = GetAngleFromPosition(element.transform.localPosition);
        float direction = angle > -90f ? 1f : -1f;
        float difference = Mathf.Abs(angle + 90f);
        int steps = Mathf.RoundToInt(difference / 45f);
        print(steps);
        
        float[] startAngle = new float[elements.Count];
        for (int i = 0; i < elements.Count; i++)
        {
            startAngle[i] = GetAngleFromPosition(elements[i].transform.localPosition);
            print($"{elements[i].name} startAngle: {startAngle[i]}");
        }
        
        float[] targetAngle = new float[elements.Count];
        for (int i = 0; i < elements.Count; i++)
        {
            targetAngle[i] = startAngle[i] + (angleDelta * direction);
            print($"{elements[i].name} targetAngle: {targetAngle[i]}");
            if (targetAngle[i] < -180f)
            {
                print(targetAngle[i]);
                targetAngle[i] = -359f;
                print(targetAngle[i]);

            }
            else if (targetAngle[i] > 0f)
            {
                targetAngle[i] = 180f;
            }
        }

        float timer = 0;
        while (timer < elementAnimSpeed)
        {
            timer = Time.unscaledTime - startTime;
            float t = Mathf.Clamp01(timer / elementAnimSpeed);
            t = Mathf.SmoothStep(0, 1, t);
            for (int i = 0; i < elements.Count; i++)
            {
                float lerpAngle = Mathf.LerpAngle(startAngle[i], targetAngle[i], t);
                float radians = lerpAngle * Mathf.Deg2Rad;
                Vector2 pos = new Vector2(radius * Mathf.Sin(radians), radius * Mathf.Cos(radians));
                elements[i].localPosition = pos;
            }
            
            yield return null;
        }
    }
}