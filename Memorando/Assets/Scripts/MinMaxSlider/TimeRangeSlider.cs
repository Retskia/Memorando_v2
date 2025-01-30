using UnityEngine;

[AddComponentMenu("UI/Time Range Slider")]
public class TimeRangeSlider : MonoBehaviour
{
    [Range(0, 1439)] // Time in minutes from 00:00 (0) to 23:59 (1439)
    public int MinTime = 0;

    [Range(0, 1439)]
    public int MaxTime = 1439;

    [Tooltip("Minimum distance in minutes between the two slider handles")]
    public int MinDistance = 360; // 6 hours

    // Returns the time range as formatted strings
    public string GetFormattedMinTime() => MinutesToTimeString(MinTime);
    public string GetFormattedMaxTime() => MinutesToTimeString(MaxTime);

    private string MinutesToTimeString(int totalMinutes)
    {
        int hours = totalMinutes / 60;
        int minutes = totalMinutes % 60;
        return $"{hours:D2}:{minutes:D2}";
    }

    private void OnValidate()
    {
        // Ensure MinTime and MaxTime satisfy the minimum distance constraint
        if (MaxTime - MinTime < MinDistance)
        {
            MaxTime = Mathf.Clamp(MinTime + MinDistance, 0, 1439);
        }
    }
}
