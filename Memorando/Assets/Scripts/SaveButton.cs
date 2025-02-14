using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveButtonScript : MonoBehaviour
{
    public CircularTimeRangeSliderUI timeRangeSlider; // Reference to the CircularTimeRangeSliderUI

    public void SaveTimeRange()
    {
        if (timeRangeSlider == null)
        {
            Debug.LogError("CircularTimeRangeSliderUI reference is missing!");
            return;
        }

        float minAngle = timeRangeSlider.GetMinAngle();
        float maxAngle = timeRangeSlider.GetMaxAngle();

        Debug.Log($"Saving MinAngle: {minAngle}, MaxAngle: {maxAngle}");
        PlayerPrefs.SetFloat("MinAngle", minAngle);
        PlayerPrefs.SetFloat("MaxAngle", maxAngle);
        PlayerPrefs.Save();
    }
}