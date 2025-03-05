using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Element : MonoBehaviour
{
    public string elementName => this.name;
    public Image img;
    public Button button;
    [SerializeField] private SkillUIAnimator skillUIAnimator;
    private void Awake()
    {
        img = GetComponent<Image>();
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        img.material = Instantiate(img.material);
        button.onClick.AddListener(() => skillUIAnimator.ScrollElement(this));
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }
}
