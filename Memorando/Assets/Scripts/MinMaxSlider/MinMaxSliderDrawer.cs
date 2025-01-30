using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TimeRangeSlider))]
public class TimeRangeSliderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TimeRangeSlider slider = (TimeRangeSlider)target;

        EditorGUILayout.LabelField("Time Range (00:00 - 23:59)", EditorStyles.boldLabel);

        // Convert MinTime and MaxTime from minutes to float (for slider range)
        float min = slider.MinTime;
        float max = slider.MaxTime;

        // Draw MinMaxSlider
        EditorGUILayout.MinMaxSlider(ref min, ref max, 0, 1439);

        // Ensure minimum distance
        if (max - min < slider.MinDistance)
        {
            max = min + slider.MinDistance;
            if (max > 1439) max = 1439;
        }

        slider.MinTime = Mathf.RoundToInt(min);
        slider.MaxTime = Mathf.RoundToInt(max);

        // Display formatted time values
        EditorGUILayout.LabelField("Min Time", slider.GetFormattedMinTime());
        EditorGUILayout.LabelField("Max Time", slider.GetFormattedMaxTime());

        // Update the serialized object
        serializedObject.ApplyModifiedProperties();
    }
}
