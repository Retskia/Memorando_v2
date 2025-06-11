using UnityEngine;
using TMPro;
using System;

public class Counter : MonoBehaviour
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

        countdownTMP.text = FormatTimeSpan(DateTime.Now, endTime);
    }

    private string FormatTimeSpan(DateTime start, DateTime end)
    {
        int years = end.Year - start.Year;
        int months = end.Month - start.Month;
        int days = end.Day - start.Day;

        if (days < 0)
        {
            months--;
            DateTime previousMonth = end.AddMonths(-1);
            days += DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month);
        }
        if (months < 0)
        {
            years--;
            months += 12;
        }

        TimeSpan remainingTime = end - start;

        return $"{years} years\n{months} months\n{days} days\n{remainingTime.Hours} hours\n{remainingTime.Minutes} minutes\n{remainingTime.Seconds} seconds";
    }
}
