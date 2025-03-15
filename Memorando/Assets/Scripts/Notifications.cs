using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using Unity.Notifications.Android;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Notifications : MonoBehaviour
{
    private string savedPhotoPath;

    void Start()
    {
        var notificationIntentData = AndroidNotificationCenter.GetLastNotificationIntent();

        if (notificationIntentData != null)
        {
            if (notificationIntentData.Notification.IntentData == "open_camera")
            {
                TakePhoto();
            }
        }
        RegisterNotificationChannel();
        ScheduleNotification();
    }

    private void RegisterNotificationChannel()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.High,
            Description = "Time to take a picture!",
            LockScreenVisibility = LockScreenVisibility.Public,
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    public void ScheduleNotification()
    {
        float minAngle = PlayerPrefs.GetFloat("MinAngle", 300f);
        float maxAngle = PlayerPrefs.GetFloat("MaxAngle", 120f);
        int minMinutes = AngleToTimeMinutes(minAngle);
        int maxMinutes = AngleToTimeMinutes(maxAngle);

        if (minMinutes > maxMinutes)
        {
            maxMinutes += 1440; // Handle past-midnight cases
        }

        AndroidNotificationCenter.CancelAllScheduledNotifications();
        AndroidNotificationCenter.CancelAllDisplayedNotifications();

        DateTime now = DateTime.Now;
        int futureDaysGenerated = 7;
        for (int i = 1; i <= futureDaysGenerated; i++)
        {
            DateTime futureDate = now.Date.AddDays(i);
            UnityEngine.Random.InitState(futureDate.Year * 10000 + futureDate.Month * 100 + futureDate.Day);
            int randomMinutes = UnityEngine.Random.Range(minMinutes, maxMinutes) % 1440;
            DateTime notificationTime = futureDate.AddMinutes(randomMinutes);

            var notification = new AndroidNotification
            {
                Title = "Memorando",
                //Text = "Time to take a picture!",
                Text = notificationTime.ToString("yyyy-MM-dd HH:mm:ss"),
                SmallIcon = "icon",
                LargeIcon = "logo",
                FireTime = notificationTime,
                IntentData = "open_camera" // This tells the app what to do
            };

            AndroidNotificationCenter.SendNotification(notification, "channel_id");
        }

        Debug.Log("Notifications scheduled.");
    }

    private int AngleToTimeMinutes(float angle)
    {
        const int TotalMinutesInDay = 1440;
        return Mathf.RoundToInt((angle + 360f) % 360f / 360f * TotalMinutesInDay);
    }

    private void TakePhoto()
    {
        if (NativeCamera.IsCameraBusy())
        {
            Debug.Log("Camera is busy");
            return;
        }

        NativeCamera.TakePicture((path) =>
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.Log("Operation cancelled");
                return;
            }

            Debug.Log("Photo saved at: " + path);

            // Move the image to persistent storage
            savedPhotoPath = Path.Combine(Application.persistentDataPath, "saved_photo.jpg");
            File.Copy(path, savedPhotoPath, true);

            // Save the path for later use
            PlayerPrefs.SetString("SavedPhotoPath", savedPhotoPath);
            PlayerPrefs.Save();

        }, maxSize: 1024);
    }
}
