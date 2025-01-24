using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionSelector : MonoBehaviour
{
    public TMPro.TextMeshProUGUI optionText;
    public string[] options = { "1 Year", "2 years", "3 years", "4 years", "5 years", "6 years" };
    private int currentIndex = 0;

    void Start()
    {
        UpdateOption();
    }


    public void NavigateLeft()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateOption();
        }
    }

    public void NavigateRight()
    {
        if (currentIndex < options.Length - 1)
        {
            currentIndex++;
            UpdateOption();
        }
    }

    private void UpdateOption()
    {
        optionText.text = options[currentIndex];
    }

    public void SaveOption()
    {
        PlayerPrefs.SetInt("SelectedOption", currentIndex);
        PlayerPrefs.Save();

        Debug.Log("Option Saved: " + options[currentIndex]);
    }
}
