using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ElementBtnAnimator : MonoBehaviour
{
    public float radius;
    public E_ArrangeType Arrangement;
    public List<Transform> elements;

    private void Awake()
    {
    }

    private void OnValidate()
    {
        elements = new();
        int count = transform.childCount;
        for (int i = 0; i < count; i++) elements.Add(transform.GetChild(i).transform);
        ArrangeElements(Arrangement);
    }

    private void ArrangeElements(E_ArrangeType arrangeType = E_ArrangeType.Circular)
    {
        float angle = arrangeType switch
        {
            E_ArrangeType.Circular => 360f / elements.Count,
            E_ArrangeType.SemiCircularL => -180f / elements.Count,
            E_ArrangeType.SemiCircularR => 180f / elements.Count,
            _=> 0f
        };
        float sectionAngle = 0;
        for (int i = 0; i < elements.Count; i++)
        {
            var radians = sectionAngle * Mathf.Deg2Rad;
            Vector2 pos = new Vector2(radius * Mathf.Sin(radians), radius * Mathf.Cos(radians));
            elements[i].localPosition = pos;
            sectionAngle += angle;
        }
    }
    
    
}

public enum E_ArrangeType
{
    Circular,
    SemiCircularL,
    SemiCircularR
}