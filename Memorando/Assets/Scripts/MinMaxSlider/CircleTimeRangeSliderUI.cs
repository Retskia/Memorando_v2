using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class CircularTimeRangeSliderUI : MonoBehaviour
{
    [Header("UI Components")]
    public RectTransform CircleBackground; // The circular background
    public RectTransform MinHandle;       // The minimum handle
    public RectTransform MaxHandle;       // The maximum handle
    public RectTransform FillArea;        // The fill area (circular arc fill)
    public TextMeshProUGUI MinTimeText;   // Text for minimum time
    public TextMeshProUGUI MaxTimeText;   // Text for maximum time
    public TextMeshProUGUI DebugLabelA;   // DELETE MEEEE

    [Header("Settings")]
    public float Radius = 100f;           // Radius of the circle
    public float StartingMinAngle = 300f;
    public float StartingMaxAngle = 120f;
    public int MinDistanceInMinutes = 360; // Minimum distance in minutes (6 hours)

    private const int TotalMinutesInDay = 1440; // Total minutes in a day (24 * 60)

    private float _minAngle;
    private float _maxAngle;


    private void Start()
    {
        // Initialize angles
        _minAngle = StartingMinAngle;
        _maxAngle = StartingMaxAngle;

        UpdateHandlesAndFill();
        LoadTimeRange();
    }

    private void UpdateDebugText(List<string> messages)
    {
        string msg = "";
        msg += "_minAngle: " + _minAngle + "\n";
        msg += "_maxAngle: " + _maxAngle + "\n";

        if (messages != null)
        {
            msg += string.Join("\n", messages);
        }

        DebugLabelA.SetText(msg);
    }

    public void OnMinHandleDragged()
    {
        Vector2 position = GetPointerPosition();
        Vector2 localPosition = GetLocalPointerPosition(position);
        _minAngle = GetAngleFromPosition(localPosition);
        //ClampMinHandle();
        UpdateHandlesAndFill();
    }

    public void OnMaxHandleDragged()
    {
        Vector2 position = GetPointerPosition();
        Vector2 localPosition = GetLocalPointerPosition(position);
        _maxAngle = GetAngleFromPosition(localPosition);
        //ClampMaxHandle();
        UpdateHandlesAndFill();
    }

    public void OnMinHandleReleased()
    {
        ClampMinHandle();
        SaveTimeRange();
    }

    public void OnMaxHandleReleased()
    {
        ClampMaxHandle();
        SaveTimeRange();
    }

    private void ClampMinHandle()
    {
        float minDistanceAngle = MinDistanceInMinutes * 360f / TotalMinutesInDay;
        float biasMaxAngle;

        if (_maxAngle < _minAngle)
            biasMaxAngle = _maxAngle + 360f;
        else
            biasMaxAngle = _maxAngle;

        DebugLabelA.SetText(Mathf.DeltaAngle(_minAngle, biasMaxAngle).ToString());
        if (Mathf.DeltaAngle(_minAngle, biasMaxAngle) < minDistanceAngle)
        {
            _minAngle = (biasMaxAngle - minDistanceAngle) % 360f;
        }
    }

    private void ClampMaxHandle()
    {
        float minDistanceAngle = MinDistanceInMinutes * 360f / TotalMinutesInDay;
        DebugLabelA.SetText(Mathf.DeltaAngle(_minAngle, _maxAngle).ToString());
        if (Mathf.DeltaAngle(_maxAngle, _minAngle) > -minDistanceAngle)
        {
            _maxAngle = (_minAngle + minDistanceAngle) % 360f;
        }
    }

    private Vector2 GetPointerPosition()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Touch touch = Input.GetTouch(0);
            return touch.position;
        }
        return new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }

    private float GetAngleFromPosition(Vector2 localPosition)
    {
        float angle = Mathf.Atan2(localPosition.x, localPosition.y) * Mathf.Rad2Deg;

        return (angle + 360f) % 360f; // Normalize to 0-360 degrees
    }

    private Vector2 GetLocalPointerPosition(Vector2 screenPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            CircleBackground,
            screenPosition,
            null, // Use the default camera for UI
            out Vector2 localPosition
        );
        return localPosition;
    }

    private void UpdateHandlesAndFill()
    {
        // Get desired positions
        Vector2 desiredMinPosition = GetPositionOnCircle(_minAngle);
        Vector2 desiredMaxPosition = GetPositionOnCircle(_maxAngle);

        int minTimeLabelMinutes = AngleToTimeMinutes(_minAngle);
        int maxTimeLabelMinutes = AngleToTimeMinutes(_maxAngle);

        // Check minimum gap
        //if (maxTimeLabelMinutes < minTimeLabelMinutes + MinDistanceInMinutes)
        //{
        //    ClampMaxHandle();
        //ClampToMinHandle}

        // Update handle positions if within minimum range
        MinHandle.localPosition = desiredMinPosition;
        MaxHandle.localPosition = desiredMaxPosition;

        // Update fill area
        UpdateFill();

        // Update time texts
        MinTimeText.text = MinutesToTimeString(minTimeLabelMinutes);
        MaxTimeText.text = MinutesToTimeString(maxTimeLabelMinutes);

    }

    private Vector2 GetPositionOnCircle(float angle)
    {
        float radians = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Sin(radians), Mathf.Cos(radians)) * Radius;
    }

    private void UpdateFill()
    {
        float startAngle = _minAngle;
        float endAngle = _maxAngle;

        FillArea.localRotation = Quaternion.Euler(0, 0, -startAngle);

        float fillAmount;
        if (endAngle >= startAngle)
            fillAmount = (endAngle - startAngle) / 360f;
        else
            fillAmount = (endAngle + 360f - startAngle) / 360f;

        FillArea.GetComponent<UnityEngine.UI.Image>().fillAmount = fillAmount;
    }

    private string MinutesToTimeString(int totalMinutes)
    {
        int hours = totalMinutes / 60;
        int minutes = totalMinutes % 60;
        return $"{hours:D2}:{minutes:D2}";
    }
    private int AngleToTimeMinutes(float angle)
    {
        return Mathf.RoundToInt((angle + 360f) % 360f / 360f * TotalMinutesInDay);
    }

    private void SaveTimeRange()
    {
        Debug.Log($"Saving MinAngle: {_minAngle}, MaxAngle: {_maxAngle}");
        PlayerPrefs.SetFloat("MinAngle", _minAngle);
        PlayerPrefs.SetFloat("MaxAngle", _maxAngle);
        PlayerPrefs.Save();
    }

    private void LoadTimeRange()
    {
        _minAngle = PlayerPrefs.GetFloat("MinAngle", _minAngle);
        _maxAngle = PlayerPrefs.GetFloat("MaxAngle", _maxAngle);
        Debug.Log($"Loaded MinAngle: {_minAngle}, MaxAngle: {_maxAngle}");
        UpdateHandlesAndFill();

        _minAngle = PlayerPrefs.GetFloat("MinAngle", StartingMinAngle);
        _maxAngle = PlayerPrefs.GetFloat("MaxAngle", StartingMaxAngle);
        Debug.Log($"Loaded MinAngle: {_minAngle}, MaxAngle: {_maxAngle}");
        UpdateHandlesAndFill();
    }

    private void OnApplicationQuit()
    {
        SaveTimeRange();
    }

    private void OnDisable()
    {
        SaveTimeRange();
    }
}