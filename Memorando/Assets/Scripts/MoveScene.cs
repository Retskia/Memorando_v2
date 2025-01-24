using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveScene : MonoBehaviour
{

    void Start()
    {
        StartCoroutine(MoveToNext());
    }

    IEnumerator MoveToNext()
    {
        yield return new WaitForSeconds(2);

        if (PlayerPrefs.GetInt("HasCompletedLaunch", 0) == 1)
        {
            SceneManager.LoadScene("HomeScene");
        }
        else 
        {
            SceneManager.LoadScene("FirstLaunchScene");
        }
    }
}
