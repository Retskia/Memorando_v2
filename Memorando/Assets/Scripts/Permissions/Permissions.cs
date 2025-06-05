using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class permissionsAll : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(RequestAllPermissions());
    }

    IEnumerator RequestAllPermissions()
    {
        // Request CAMERA permission
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
            yield return new WaitForSeconds(0.5f); // Small delay to ensure the dialog shows
        }

        // Request NOTIFICATIONS permission (Android 13+)
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
            yield return new WaitForSeconds(0.5f);
        }

        // Request READ_MEDIA_IMAGES permission (for accessing saved images on Android 13+)
        if (!Permission.HasUserAuthorizedPermission("android.permission.READ_MEDIA_IMAGES"))
        {
            Permission.RequestUserPermission("android.permission.READ_MEDIA_IMAGES");
            yield return new WaitForSeconds(0.5f);
        }

        if (!Permission.HasUserAuthorizedPermission("android.permission.ACCESS_FINE_LOCATION"))
        {
            Permission.RequestUserPermission("android.permission.ACCESS_FINE_LOCATION");
            yield return new WaitForSeconds(0.5f);
        }
    }

}

