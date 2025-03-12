using UnityEngine;
using TMPro;
using System;

public class counter : MonoBehaviour
{
    public TMP_Text countdownTMP;
    public GameObject objectToHide;

    private DateTime endTime;

    void Start()
    {
        LoadEndTime();
        InvokeRepeating("UpdateCountdown", 0f, 1f);
    }

    void LoadEndTime()
    {
        if (PlayerPrefs.HasKey("EndTimestamp"))
        {
            long savedTime = Convert.ToInt64(PlayerPrefs.GetString("EndTimestamp"));
            endTime = DateTime.FromBinary(savedTime);
        }
        else
        {
            int selectedYears = PlayerPrefs.GetInt("SelectedOption", 0) + 1;
            endTime = DateTime.Now.AddYears(selectedYears);
            PlayerPrefs.SetString("EndTimestamp", endTime.ToBinary().ToString());
            PlayerPrefs.Save();
        }
    }

    void UpdateCountdown()
    {
        TimeSpan remainingTime = endTime - DateTime.Now;

        if (remainingTime.TotalSeconds <= 0)
        {
            CancelInvoke("UpdateCountdown");
            countdownTMP.text = "0 years\n0 months\n0 days\n0 hours\n0 minutes\n0 seconds";

            if (objectToHide != null)
                objectToHide.SetActive(false);

            return;
        }

        countdownTMP.text = FormatTimeSpan(remainingTime);
    }

    private string FormatTimeSpan(TimeSpan timeSpan)
    {
        int years = timeSpan.Days / 365;
        int months = (timeSpan.Days % 365) / 30;
        int days = (timeSpan.Days % 365) % 30;

        return $"{years} years\n{months} months\n{days} days\n{timeSpan.Hours} hours\n{timeSpan.Minutes} minutes\n{timeSpan.Seconds} seconds";
    }
}
