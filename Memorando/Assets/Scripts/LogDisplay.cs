using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogDisplay : MonoBehaviour
{
    public TMP_Text logText;

    public void ShowLogs()
    {
        logText.text = string.Join("\n", NotificationLog.logs);
    }
}
