using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButtonAnim : MonoBehaviour
{
    public float radius;
    public bool SkillMenuActive;
    public List<Button> elements;
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
        for (int i = 0; i < count; i++) elements.Add(transform.GetChild(i).GetComponent<Button>());
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

    private float GetAngleFromPosition(Vector2 pos) => Mathf.Atan2(pos.x, pos.y) * Mathf.Rad2Deg;

    private float ElementScaler(float angle)
    {
        float difference = Mathf.Abs(angle + 90f);
        float scale = difference switch
        {
            0f => 1.25f,
            45f => 1.05f,
            _ => 0.85f
        };
        return scale;
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
        SkillMenuActive = !SkillMenuActive;
        foreach (Button b in elements) b.interactable = SkillMenuActive;
        Vector2 startPos = transform.localPosition;
        Vector2 targetPos = SkillMenuActive ? targetPosition : startPosition;
        float startScale = transform.localScale.x;
        float finalScale = SkillMenuActive ? targetScale : 1f;

        float timer = 0;
        float startTime = Time.unscaledTime;
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
        float[] startAngle = new float[elements.Count];
        float[] targetAngle = ElementTargetAngles();
        float[] startScale = new float[elements.Count];
        float[] targetScale = new float[elements.Count];
        for (int i = 0; i < elements.Count; i++)
        {
            float currentAngle = GetAngleFromPosition(elements[i].transform.localPosition);
            startAngle[i] = currentAngle;
            startScale[i] = elements[i].transform.localScale.x;
            targetScale[i] = ElementScaler(targetAngle[i]);
        }

        yield return StartCoroutine(Animate(startAngle, targetAngle, startScale, targetScale, elementAnimSpeed));
    }

    private IEnumerator Animate(float[] startAngle, float[] targetAngle, float[] startScale, float[] targetScale, float animSpeed)
    {
        yield return null;
        float startTime = Time.unscaledTime;
        float timer = 0;
        while (timer < animSpeed)
        {
            timer = Time.unscaledTime - startTime;
            float t = Mathf.Clamp01(timer / animSpeed);
            t = Mathf.SmoothStep(0, 1, t);
            for (int i = 0; i < elements.Count; i++)
            {
                float lerpAngle = Mathf.LerpAngle(startAngle[i], targetAngle[i], t);
                float lerpScale = Mathf.Lerp(startScale[i], targetScale[i], t);
                float radians = lerpAngle * Mathf.Deg2Rad;
                Vector2 pos = new Vector2(radius * Mathf.Sin(radians), radius * Mathf.Cos(radians));
                elements[i].transform.localPosition = pos;
                elements[i].transform.localScale = new Vector2(lerpScale, lerpScale);
            }

            yield return null;
        }
    }

    public void ScrollElement(Element element) => StartCoroutine(ElementScroll(element));

    private IEnumerator ElementScroll(Element selectedElement)
    {
        foreach (Button b in elements) b.interactable = false;
        float angleDelta = -180f / (elements.Count - 1);
        float angle = GetAngleFromPosition(selectedElement.transform.localPosition);
        angle = angle > 0 ? -angle : angle;
        float direction = angle > -90f ? 1f : -1f;

        float[] startAngle = new float[elements.Count];
        float[] targetAngle = new float[elements.Count];
        float[] startScale = new float[elements.Count];
        float[] targetScale = new float[elements.Count];
        int dirtyIndex = 0;

        for (int i = 0; i < elements.Count; i++)
        {
            startAngle[i] = GetAngleFromPosition(elements[i].transform.localPosition);
            startAngle[i] = startAngle[i] > 0 ? -startAngle[i] : startAngle[i];
            
            targetAngle[i] = startAngle[i] + (angleDelta * direction);
            if (targetAngle[i] < -180f)
            {
                dirtyIndex = i;
                targetAngle[i] = -359f;
            }
            else if (targetAngle[i] > 0f) targetAngle[i] = 180f;
            
            startScale[i] = elements[i].transform.localScale.x;
            targetScale[i] = ElementScaler(targetAngle[i]);
        }

        yield return StartCoroutine(Animate(startAngle, targetAngle, startScale, targetScale, elementAnimSpeed));

        float dirtyAngle = GetAngleFromPosition(elements[dirtyIndex].transform.localPosition);
        if (dirtyAngle > 0f)
        {
            float radians = 0f;
            Vector2 pos = new Vector2(radius * Mathf.Sin(radians), radius * Mathf.Cos(radians));
            elements[dirtyIndex].transform.localPosition = pos;
        }

        foreach (Button b in elements) b.interactable = true;
    }
}