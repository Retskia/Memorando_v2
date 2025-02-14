using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetButton : MonoBehaviour
{
    public CircularTimeRangeSliderUI timeRangeSlider; // Reference to the CircularTimeRangeSliderUI

    public void ResetTimeRange()
    {
        if (timeRangeSlider == null)
        {
            Debug.LogError("CircularTimeRangeSliderUI reference is missing!");
            return;
        }

        // Reset angles to starting values
        timeRangeSlider.ResetToDefault();
        Debug.Log("Time range reset to default values.");
    }
}
