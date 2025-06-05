using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using Unity.Notifications.Android;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Notifications : MonoBehaviour
{
    void Start()
    {
        var notificationIntentData = AndroidNotificationCenter.GetLastNotificationIntent();

        if (notificationIntentData != null)
        {
            string intentData = notificationIntentData.Notification.IntentData;

            if (!string.IsNullOrEmpty(intentData) && intentData.StartsWith("open_camera"))
            {
                string[] parts = intentData.Split('|');
                if (parts.Length == 2)
                {
                    string fireTimeStr = parts[1];
                    if (DateTime.TryParseExact(fireTimeStr, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime fireTime))
                    {
                        if ((DateTime.Now - fireTime).TotalMinutes <= 5)
                        {
                            TakePhoto();
                        }
                        else
                        {
                            Debug.Log("Time expired! You can’t take the photo anymore.");
                        }
                    }
                    else
                    {
                        Debug.Log("Failed to parse fire time from IntentData.");
                    }
                }
                else
                {
                    Debug.Log("Invalid intent format.");
                }
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

            // Save notification time for checking expiration
            PlayerPrefs.SetString("LastNotificationTime", notificationTime.ToString("yyyy-MM-dd HH:mm:ss"));
            PlayerPrefs.Save();

            var notification = new AndroidNotification
            {
                Title = "Memorando",
                Text = "You have 5 minutes to take a picture!",
                SmallIcon = "icon",
                LargeIcon = "logo",
                FireTime = notificationTime,
                IntentData = "open_camera|" + notificationTime.ToString("yyyy-MM-dd HH:mm:ss")
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

    private bool IsNotificationValid()
    {
        string lastNotificationTimeStr = PlayerPrefs.GetString("LastNotificationTime", "");
        if (string.IsNullOrEmpty(lastNotificationTimeStr))
        {
            return false; // No valid notification time
        }

        DateTime lastNotificationTime = DateTime.ParseExact(lastNotificationTimeStr, "yyyy-MM-dd HH:mm:ss", null);
        TimeSpan timeElapsed = DateTime.Now - lastNotificationTime;

        return timeElapsed.TotalMinutes <= 5; // Valid only if within 5 minutes
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

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string newFilePath = Path.Combine(Application.persistentDataPath, "photo_" + timestamp + ".jpg");
            File.Copy(path, newFilePath, true);

            int imageCount = PlayerPrefs.GetInt("ImageCount", 0);
            PlayerPrefs.SetString("ImagePath_" + imageCount, newFilePath);
            PlayerPrefs.SetInt("ImageCount", imageCount + 1);
            PlayerPrefs.Save();

            Debug.Log("Photo saved at: " + newFilePath);

        }, maxSize: 1024);
    }
}
