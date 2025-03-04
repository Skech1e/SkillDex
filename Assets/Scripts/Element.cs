using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Element : MonoBehaviour
{
    public string elementName => this.name;
    public Image img;
    private Button button;
    [SerializeField] private SkillButtonAnim skillButtonAnim;
    private void Awake()
    {
        img = GetComponent<Image>();
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(() => skillButtonAnim.ScrollElement(this));
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }
}
