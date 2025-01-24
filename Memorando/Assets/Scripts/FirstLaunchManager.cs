using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstLaunchManager : MonoBehaviour
{
    public void ConfirmChoice()
    {
       
        PlayerPrefs.SetInt("HasCompletedFirstLaunch", 1);
        PlayerPrefs.Save(); 

        
        SceneManager.LoadScene("HomeScene"); 
    }
}



