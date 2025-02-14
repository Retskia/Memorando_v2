using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationPermission : MonoBehaviour
{
    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            RequestPermissionAndroid();
        }
    }

    private void RequestPermissionAndroid()
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaObject permissionChecker = new AndroidJavaObject("androidx.core.app.ActivityCompat");

            string notificationPermission = "android.permission.POST_NOTIFICATIONS";

            int permissionCheck = activity.Call<int>("checkSelfPermission", notificationPermission);
            if (permissionCheck != 0)
            {
                activity.Call("requestPermissions", new string[] { notificationPermission }, 1);
            }
        }
    }
}

