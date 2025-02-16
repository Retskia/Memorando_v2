using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Notifications.Android;
using UnityEngine;

public class Notifications : MonoBehaviour
{
    void Start()
    {
        // Check if the app was opened from a notification
        var notificationIntentData = AndroidNotificationCenter.GetLastNotificationIntent();

        if (notificationIntentData != null)
        {
            if (notificationIntentData.Notification.IntentData == "open_camera")
            {
                OpenCamera();
            }
        }

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
        var notification = new AndroidNotification
        {
            Title = "Memorando",
            Text = "Time to take a picture",
            SmallIcon = "icon",
            LargeIcon = "logo",
            FireTime = notificationTime,
            IntentData = "open_camera" // This allows us to detect when it's clicked
        };

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

    private void OpenCamera()
    {
        try
        {
            // Open camera using Android intent
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
            AndroidJavaClass intentClass = new AndroidJavaClass("android.provider.MediaStore");

            intent.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_IMAGE_CAPTURE"));
            currentActivity.Call("startActivity", intent);

            Debug.Log("Camera opened from notification");
        }
        catch (Exception e)
        {
            Debug.LogError("Error opening camera: " + e.Message);
        }
    }
}