using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeRangeSliderUI : MonoBehaviour
{
    [Header("UI Components")]
    public RectTransform FillArea; // Fill image area
    public Slider BackgroundSlider; // The background slider
    public Slider MinHandleSlider; // Slider for the minimum handle
    public Slider MaxHandleSlider; // Slider for the maximum handle
    public TextMeshProUGUI MinTimeText; // Text to display the minimum time
    public TextMeshProUGUI MaxTimeText; // Text to display the maximum time

    [Header("Settings")]
    public int MinDistanceInMinutes = 360; // Minimum distance in minutes (6 hours)

    private const int TotalMinutesInDay = 1440; // Total minutes in a day (24 * 60)

    private void Start()
    {
        // Initialize sliders
        MinHandleSlider.minValue = 0;
        MinHandleSlider.maxValue = TotalMinutesInDay;
        MaxHandleSlider.minValue = 0;
        MaxHandleSlider.maxValue = TotalMinutesInDay;

        // Set initial values
        MinHandleSlider.value = 0;
        MaxHandleSlider.value = TotalMinutesInDay;

        UpdateFillAndTimeDisplay();
    }

    public void OnMinHandleValueChanged()
    {
        // Ensure MinHandleSlider doesn't overlap MaxHandleSlider minus the minimum distance
        if (MinHandleSlider.value > MaxHandleSlider.value - MinDistanceInMinutes)
        {
            MinHandleSlider.value = MaxHandleSlider.value - MinDistanceInMinutes;
        }

        UpdateFillAndTimeDisplay();
    }

    public void OnMaxHandleValueChanged()
    {
        // Ensure MaxHandleSlider doesn't overlap MinHandleSlider plus the minimum distance
        if (MaxHandleSlider.value < MinHandleSlider.value + MinDistanceInMinutes)
        {
            MaxHandleSlider.value = MinHandleSlider.value + MinDistanceInMinutes;
        }

        UpdateFillAndTimeDisplay();
    }

    private void UpdateFillAndTimeDisplay()
    {
        // Update time display
        MinTimeText.text = MinutesToTimeString((int)MinHandleSlider.value);
        MaxTimeText.text = MinutesToTimeString((int)MaxHandleSlider.value);

        // Update fill position and size
        float fillStart = MinHandleSlider.value / TotalMinutesInDay;
        float fillEnd = MaxHandleSlider.value / TotalMinutesInDay;

        FillArea.anchorMin = new Vector2(fillStart, FillArea.anchorMin.y);
        FillArea.anchorMax = new Vector2(fillEnd, FillArea.anchorMax.y);
    }

    private string MinutesToTimeString(int totalMinutes)
    {
        int hours = totalMinutes / 60;
        int minutes = totalMinutes % 60;
        return $"{hours:D2}:{minutes:D2}";
    }
}
