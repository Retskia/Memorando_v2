using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using Unity.Notifications.Android;
using UnityEngine;

public class NotificationOpener : MonoBehaviour
{
    private string photoPath;

    void Start()
    {
        var notificationIntentData = AndroidNotificationCenter.GetLastNotificationIntent();

        if (notificationIntentData != null)
        {
            if (notificationIntentData.Notification.IntentData == "open_camera")
            {
                OpenCamera();
            }
        }

        //RegisterNotificationChannel();
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

    private string DateToFilename(DateTime date)
    {
        return "memorando_" + date.ToString("yyyyMMdd") + ".jpg";
    }

    private void OpenCamera()
    {
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
            AndroidJavaClass intentClass = new AndroidJavaClass("android.provider.MediaStore");

            intent.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_IMAGE_CAPTURE"));

            // Get file path and use Android FileProvider for compatibility
            photoPath = Path.Combine(Application.persistentDataPath, DateToFilename(DateTime.Now));
            PlayerPrefs.SetString("LastPhotoPath", photoPath);

            // Create a file object in Java
            AndroidJavaObject file = new AndroidJavaObject("java.io.File", photoPath);
            AndroidJavaClass fileProvider = new AndroidJavaClass("androidx.core.content.FileProvider");
            AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");

            string authority = context.Call<string>("getPackageName") + ".provider";
            AndroidJavaObject uri = fileProvider.CallStatic<AndroidJavaObject>("getUriForFile", context, authority, file);

            intent.Call<AndroidJavaObject>("putExtra", "output", uri);
            intent.Call<AndroidJavaObject>("addFlags", 3); // Grant temporary permissions

            currentActivity.Call("startActivity", intent);

            Debug.Log("Camera opened from notification, saving image to: " + photoPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Error opening camera: " + e.Message);
        }
    }
}
