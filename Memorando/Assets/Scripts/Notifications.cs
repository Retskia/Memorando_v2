using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;

public class Notifications : MonoBehaviour
{
    
    void Start()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notification",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        var notification = new AndroidNotification();
        notification.Title = "Memorando";
        notification.Text = "Time to take a picture";
        notification.SmallIcon = "icon";
        notification.LargeIcon = "logo";

        //Timed notification
        notification.FireTime = System.DateTime.Now.AddMinutes(1);

        //Send notification
        AndroidNotificationCenter.SendNotification(notification, "channel_id");
        
    }

    
    void Update()
    {
        
    }
}
