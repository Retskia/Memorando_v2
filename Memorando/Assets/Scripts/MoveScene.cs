using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveScene : MonoBehaviour
{
    public TextMeshProUGUI DebugLabelA;

    void Start()
    {

        StartCoroutine(MoveToNext());
        DebugLabelA.SetText(PlayerPrefs.GetInt("HasCompletedFirstLaunch", 0).ToString());
    }

    IEnumerator MoveToNext()
    {
        yield return new WaitForSeconds(2);

        if (PlayerPrefs.GetInt("HasCompletedFirstLaunch", 0) == 1)
        {
            SceneManager.LoadScene("HomeScene");
        }
        else 
        {
            SceneManager.LoadScene("FirstLaunchScene");
        }
    }
}
