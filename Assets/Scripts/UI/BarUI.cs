using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// For creating size changing bars. Eg: health bar
/// </summary>
public class BarUI : MonoBehaviour
{
    /// <summary>
    /// The rectTransform of the bar
    /// </summary>
    [SerializeField]
    protected RectTransform _bar = null;
    /// <summary>
    /// The percentage between 0 & 1 for the length of the bar
    /// </summary>
    [Range(0, 1)]
    [SerializeField]
    protected float _percentage = 1;
    /// <summary>
    /// The original starting width of the bar
    /// </summary>
    private float _originalWidth = 0;
    /// <summary>
    /// For changing the percentage of the bar. Automatically re-sizes the bar
    /// </summary>
    public float Percentage
    {
        get => _percentage;
        set
        {
            _percentage = Mathf.Clamp(value, 0, 1);
            //Adjust the size of the bar
            _bar.pivot = Vector2.zero;
            _bar.sizeDelta = new Vector2(_originalWidth * _percentage, _bar.sizeDelta.y);
        }
    }
    /// <summary>
    /// Gets the original width of the bar
    /// </summary>
    private void Awake()
    {
        _originalWidth = _bar.sizeDelta.x;
    }
}