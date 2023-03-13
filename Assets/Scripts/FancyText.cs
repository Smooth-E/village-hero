using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Text))]
public class FancyText : MonoBehaviour, IPointerDownHandler
{

    Text textComponent;
    public string textString;
    bool spellingCoroutineInProgress = false;
    [HideInInspector] public bool textSpelled = false;
    [HideInInspector] public bool spellImidiately = false;
    const string spellingCoroutineName = "spellingCoroutine";
    const string setNiceTextSizeCoroutineName = "setNiceTextSizeCoroutine";
    public float spellingDelay = .01f;
    bool textSizeSet = false;
    bool isActive;
    bool retype = false;

    void Start()
    {
        textComponent = GetComponent<Text>();
        textString = textComponent.text;
        StartCoroutine(setNiceTextSizeCoroutineName);
    }

    IEnumerator setNiceTextSizeCoroutine()
    {
        yield return null;
        int ceachedSize = (int)(textComponent.cachedTextGenerator.fontSizeUsedForBestFit / textComponent.canvas.scaleFactor);
        textComponent.resizeTextForBestFit = false;
        textComponent.fontSize = ceachedSize;
        textSizeSet = true;
    }

    IEnumerator spellingCoroutine()
    {
        retype = false;
        spellingCoroutineInProgress = true;
        textComponent.text = "";
        bool finishedByRetype = false;
        foreach(char symbol in textString)
        {
            if (retype)
            {
                finishedByRetype = true;
                StartCoroutine(spellingCoroutine());
                break;
            }
            textComponent.text += symbol;
            if (spellImidiately)
            {
                textComponent.text = textString;
                break;
            }
            if (!isActive) break;
            yield return new WaitForSeconds(spellingDelay);
        }
        if (!finishedByRetype)
        {
            textSpelled = true;
            spellingCoroutineInProgress = false;
        }
    }

    void Update()
    {
        if (textSizeSet)
        {
            isActive = GetComponent<RectTransform>().lossyScale.x > 0 && GetComponent<RectTransform>().lossyScale.y > 0;
            if (isActive && !spellingCoroutineInProgress && !textSpelled) StartCoroutine(spellingCoroutineName);
            else if (!isActive)
            {
                textComponent.text = "";
                textSpelled = false;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        spellImidiately = true;
    }

    IEnumerator changeTextCoruotine(string newText)
    {
        yield return null;
        textString = newText;
        GetComponent<RectTransform>().localScale = Vector2.zero;
        yield return null;
        GetComponent<RectTransform>().localScale = Vector2.one;
    }

    public void ChangeText(string newText)
    {
        StartCoroutine(changeTextCoruotine(newText));
    }
}
