using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

[RequireComponent(typeof(Text))]
public class FancyText : MonoBehaviour, IPointerDownHandler
{

    [FormerlySerializedAs("textString")] public string TextString;
    public float SpellingDelay = 0.03333334f;
    
    [HideInInspector] public bool TextSpelled;
    [HideInInspector] public bool SpellImmediately;

    private RectTransform _rectTransform;
    private bool _spellingCoroutineInProgress = false;
    private Text _textComponent;
    private bool _textSizeSet;
    private bool _isActive;
    private bool _retype;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _textComponent = GetComponent<Text>();
        TextString = _textComponent.text;
        StartCoroutine(SetNiceTextSizeCoroutine());
    }

    private IEnumerator SetNiceTextSizeCoroutine()
    {
        yield return null;
        
        var bestFitSize = _textComponent.cachedTextGenerator.fontSizeUsedForBestFit;
        var scaleFactor = _textComponent.canvas.scaleFactor;
        var cachedSize = (int)(bestFitSize / scaleFactor);
        
        _textComponent.resizeTextForBestFit = false;
        _textComponent.fontSize = cachedSize;
        _textSizeSet = true;
    }

    IEnumerator SpellingCoroutine()
    {
        _retype = false;
        _spellingCoroutineInProgress = true;
        _textComponent.text = "";
        var finishedByRetype = false;
        
        foreach(char symbol in TextString)
        {
            if (_retype)
            {
                finishedByRetype = true;
                StartCoroutine(SpellingCoroutine());
                break;
            }
            
            _textComponent.text += symbol;
            if (SpellImmediately)
            {
                _textComponent.text = TextString;
                break;
            }
            
            if (!_isActive) 
                break;
            
            yield return new WaitForSeconds(SpellingDelay);
        }
        if (!finishedByRetype)
        {
            TextSpelled = true;
            _spellingCoroutineInProgress = false;
        }
    }

    void Update()
    {
        if (_textSizeSet)
        {
            _isActive = _rectTransform.lossyScale.x > 0 && _rectTransform.lossyScale.y > 0;
            
            if (_isActive && !_spellingCoroutineInProgress && !TextSpelled)
            {
                StartCoroutine(SpellingCoroutine());
            }
            else if (!_isActive)
            {
                _textComponent.text = "";
                TextSpelled = false;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData) =>
        SpellImmediately = true;

    IEnumerator ChangeTextCoruotine(string newText)
    {
        yield return null;
        
        TextString = newText;
        _rectTransform.localScale = Vector2.zero;
        yield return null;
        
        _rectTransform.localScale = Vector2.one;
    }

    public void ChangeText(string newText) =>
        StartCoroutine(ChangeTextCoruotine(newText));

}
