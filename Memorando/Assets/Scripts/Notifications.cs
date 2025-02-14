using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Notifications.Android;
using UnityEngine;

public class Notifications : MonoBehaviour
{
    void Start()
    {
        ScheduleNotification();
    }

    public void ScheduleNotification()
    {
        // Load the min and max angles from PlayerPrefs
        float minAngle = PlayerPrefs.GetFloat("MinAngle", 300f);
        float maxAngle = PlayerPrefs.GetFloat("MaxAngle", 120f);

        // Convert angles to time values in minutes
        int minMinutes = AngleToTimeMinutes(minAngle);
        int maxMinutes = AngleToTimeMinutes(maxAngle);

        // If min > max, it means the time range goes past midnight
        if (minMinutes > maxMinutes)
        {
            maxMinutes += 1440; // Add 24 hours in minutes to maxMinutes
        }

        // Generate a random time within the selected range
        int randomMinutes = UnityEngine.Random.Range(minMinutes, maxMinutes) % 1440;

        // Get today's date and add the random time
        DateTime now = DateTime.Now;
        DateTime notificationTime = now.Date.AddMinutes(randomMinutes);

        // If the notification time is in the past, schedule it for tomorrow
        if (notificationTime < now)
        {
            notificationTime = notificationTime.AddDays(1);
        }

        // Register the notification channel if not already done
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.High,
            Description = "Generic notification",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        // Create the notification
        var notification = new AndroidNotification();
        notification.Title = "Memorando";
        notification.Text = "Time to take a picture";
        notification.SmallIcon = "icon";
        notification.LargeIcon = "logo";
        notification.FireTime = notificationTime;

        // Cancel any previously scheduled notifications
        AndroidNotificationCenter.CancelAllScheduledNotifications();

        // Send the new notification
        AndroidNotificationCenter.SendNotification(notification, "channel_id");

        Debug.Log("Notification scheduled for: " + notificationTime);
    }

    private int AngleToTimeMinutes(float angle)
    {
        const int TotalMinutesInDay = 1440;
        return Mathf.RoundToInt((angle + 360f) % 360f / 360f * TotalMinutesInDay);
    }
}