using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

// UI component to select a number in a descrete range
public class Dial : MonoBehaviour, IMoveHandler
{
    [Header("Range")]
    [SerializeField] private TMPro.TMP_Text valueText;
    [SerializeField] private int min;
    [SerializeField] private int max;

    public int Value
    {
        get { return _value; }
        set
        {
            _value = Math.Clamp(value, min, max);
            valueText.text = useValueNames ? valueNames[_value] : "" + _value;
            // OnValueChanged?.Invoke(delta);
            onValueChanged.Invoke(_value);
        }
    }

    [SerializeField] private int _value;
    [SerializeField] private bool loop;

    [Header("Named Values")]
    [SerializeField] private bool useValueNames = true;
    [SerializeField] private string[] valueNames;
    
    [Header("Interaction")]
    [SerializeField] private bool useXAxis = true;
    [SerializeField] private Button negativeButton;
    [SerializeField] private Button positiveButton;

    // public delegate void OnValueChangedHandler(int value);
    // public event OnValueChangedHandler OnValueChanged;

    public UnityEngine.Events.UnityEvent<int> onValueChanged;

    public void Awake()
    {
        // _value = Math.Clamp(_value, min, max);
        valueText.text = useValueNames ? valueNames[_value - min] : "" + _value;

        // setup events
        if (negativeButton) negativeButton.onClick.AddListener(() => ChangeSelection(-1));
        if (positiveButton) positiveButton.onClick.AddListener(() => ChangeSelection(1));
    }

    public void OnMove(AxisEventData eventData)
    {
        int delta = (int) (useXAxis ? eventData.moveVector.x : eventData.moveVector.y); 
        ChangeSelection(delta);
    }

    private void ChangeSelection(int delta)
    {
        if (delta != 0)
        {
            Value = loop ? delta + Value : (delta + Value) % (max - min) + min;
        }
    }
}
