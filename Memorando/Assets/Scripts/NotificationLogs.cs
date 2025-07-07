using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NotificationLog
{
    public static List<string> logs = new List<string>();

    public static void AddLog(DateTime dateTime)
    {
        // Format it like "24.6.2025 18:16"
        string log = dateTime.ToString("d.M.yyyy HH:mm");

        logs.Add(log);

        // Keep only the 10 most recent
        if (logs.Count > 10)
            logs.RemoveAt(0);
    }
}
