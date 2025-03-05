using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SkillUIAnimator : MonoBehaviour
{
    public float radius;
    public bool SkillMenuActive;
    public List<Element> elements;
    public Vector2 defaultPos, targetPosition;
    public float targetScale;
    
    [Header("Skill Menu")]
    public Transform SkillMenuTransform;
    public Vector2 skillMenuDefaultPos, skillMenuTargetPos = Vector2.zero;
    public float skillBtnAnimSpeed, elementAnimSpeed, skillMenuAnimSpeed;
    public TextMeshProUGUI skillMenuTitle;

    private void Start()
    {
        defaultPos = transform.localPosition;
        skillMenuDefaultPos = SkillMenuTransform.localPosition;
    }

    private void OnValidate()
    {
        elements = new();
        int count = transform.childCount;
        for (int i = 0; i < count; i++) elements.Add(transform.GetChild(i).GetComponent<Element>());
    }

    private float[] ElementsLayoutAngles()
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

    public void SkillMenu()
    {
        SkillMenuActive = !SkillMenuActive;
        StartCoroutine(AnimateButton());
        StartCoroutine(AnimateElements());
    }

    private IEnumerator AnimateButton()
    {
        if(SkillMenuActive == false) yield return StartCoroutine(AnimateSkillMenu(SkillMenuActive));
        
        foreach (Element e in elements) e.button.interactable = SkillMenuActive;
        
        Vector2 startPos = transform.localPosition;
        Vector2 targetPos = SkillMenuActive ? targetPosition : defaultPos;
        float startScale = transform.localScale.x;
        float finalScale = SkillMenuActive ? targetScale : 1f;
        
        yield return StartCoroutine(UpdatePositionAndScale(transform, startPos, targetPos, skillBtnAnimSpeed, startScale, finalScale));
        
        if(SkillMenuActive) StartCoroutine(AnimateSkillMenu(SkillMenuActive));
    }

    private IEnumerator UpdatePositionAndScale(Transform objT, Vector2 startPos, Vector2 targetPos, float animSpeed, float startScale = 0f, float finalScale = 0f)
    {
        float timer = 0;
        float startTime = Time.unscaledTime;
        while (timer < animSpeed)
        {
            timer = Time.unscaledTime - startTime;
            float t = Mathf.Clamp01(timer / animSpeed);
            t = Mathf.SmoothStep(0, 1, t);
            float x = Mathf.Lerp(startPos.x, targetPos.x, t);
            float y = Mathf.Lerp(startPos.y, targetPos.y, t);
            objT.localPosition = new Vector2(x, y);
            
            if (startScale != finalScale)
            {
                float scale = Mathf.Lerp(startScale, finalScale, t);
                transform.localScale = new Vector3(scale, scale, scale);
            }

            yield return null;
        }
    }

    private IEnumerator AnimateSkillMenu(bool enabled)
    {
        Vector2 startPos = SkillMenuTransform.localPosition;
        Vector2 targetPos = enabled ? skillMenuTargetPos : skillMenuDefaultPos;
        yield return StartCoroutine(UpdatePositionAndScale(SkillMenuTransform, startPos, targetPos, skillMenuAnimSpeed));
    }
    

    private IEnumerator AnimateElements()
    {
        float[] startAngle = new float[elements.Count];
        float[] targetAngle = ElementsLayoutAngles();
        float[] startScale = new float[elements.Count];
        float[] targetScale = new float[elements.Count];
        for (int i = 0; i < elements.Count; i++)
        {
            float currentAngle = GetAngleFromPosition(elements[i].transform.localPosition);
            startAngle[i] = currentAngle;
            startScale[i] = elements[i].transform.localScale.x;
            targetScale[i] = ElementScaler(targetAngle[i]);
        }

        yield return StartCoroutine(LerpElements(startAngle, targetAngle, startScale, targetScale, elementAnimSpeed));
    }

    private IEnumerator LerpElements(float[] startAngle, float[] targetAngle, float[] startScale, float[] _targetScale, float animSpeed)
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
                Element thisElement = elements[i];
                float lerpAngle = Mathf.LerpAngle(startAngle[i], targetAngle[i], t);
                float lerpScale = Mathf.Lerp(startScale[i], _targetScale[i], t);
                UpdateElementTransform(thisElement.transform, lerpAngle, lerpScale);

                if (targetAngle[i] == -90f)
                {
                    thisElement.img.material.SetFloat("_Saturation", t);
                    UpdateSkillMenuTitle(thisElement.elementName);
                }
                else if (startAngle[i] == -90f) thisElement.img.material.SetFloat("_Saturation", 1 - t);
            }

            yield return null;
        }
    }

    private void UpdateElementTransform(Transform element, float angle, float scale)
    {
        float radians = angle * Mathf.Deg2Rad;
        Vector2 pos = new Vector2(radius * Mathf.Sin(radians), radius * Mathf.Cos(radians));
        element.localPosition = pos;
        element.localScale = new Vector2(scale, scale);
    }

    private void UpdateSkillMenuTitle(string elementName) => skillMenuTitle.text = elementName;
    
    public void ScrollElement(Element element) => StartCoroutine(ElementScroll(element));

    private IEnumerator ElementScroll(Element selectedElement)
    {
        float angle = GetAngleFromPosition(selectedElement.transform.localPosition);
        angle = angle > 0 ? -angle : angle;

        float direction = 0;
        if (angle > -90f) direction = 1f;
        else if (angle < -90f) direction = -1f;
        else yield break;
        
        StartCoroutine(AnimateSkillMenu(false));
        
        float angleDelta = -180f / (elements.Count - 1);
        foreach (Element e in elements) e.button.interactable = false;

        float[] startAngle = new float[elements.Count];
        float[] targetAngle = new float[elements.Count];
        float[] startScale = new float[elements.Count];
        float[] targetScale = new float[elements.Count];
        int dirtyIndex = 0;

        for (int i = 0; i < elements.Count; i++)
        {
            var ang = GetAngleFromPosition(elements[i].transform.localPosition);
            startAngle[i] = ang > 0 ? -ang : ang;
            var target = startAngle[i] + (angleDelta * direction);

            if (target < -180f)
            {
                dirtyIndex = i;
                targetAngle[i] = -359f;
            }
            else if (target > 0f) targetAngle[i] = 180f;
            else targetAngle[i] = target;

            startScale[i] = elements[i].transform.localScale.x;
            targetScale[i] = ElementScaler(targetAngle[i]);
        }

        yield return StartCoroutine(LerpElements(startAngle, targetAngle, startScale, targetScale, elementAnimSpeed));

        float dirtyAngle = GetAngleFromPosition(elements[dirtyIndex].transform.localPosition);
        if (dirtyAngle > 0f)
        {
            float radians = 0f;
            Vector2 pos = new Vector2(radius * Mathf.Sin(radians), radius * Mathf.Cos(radians));
            elements[dirtyIndex].transform.localPosition = pos;
        }

        yield return StartCoroutine(AnimateSkillMenu(true));
        foreach (Element e in elements) e.button.interactable = true;
    }
}