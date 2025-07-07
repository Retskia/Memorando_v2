using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class PhotoDisplay : MonoBehaviour
{
    public Image img;
    public Button nextButton;
    public Button prevButton;
    public TMP_Text info_text;
    private List<string> imagePaths = new List<string>();
    private int currentIndex = 0;
    private Texture2D currentTexture = null;

    void Start()
    {
        LoadImagePaths();
        if (imagePaths.Count > 0)
        {
            currentIndex = 0; // Start from the first image
            LoadImage(imagePaths[currentIndex]);
        }
    }

    private void LoadImagePaths()
    {
        imagePaths.Clear();
        int count = PlayerPrefs.GetInt("ImageCount", 0);

        for (int i = 0; i < count; i++)
        {
            string base64 = PlayerPrefs.GetString("ImageBase64_" + i, "");
            if (!string.IsNullOrEmpty(base64))
            {
                imagePaths.Add(base64);
            }
        }
    }



    private void LoadImage(string base64)
    {
        if (currentTexture != null)
        {
            Destroy(currentTexture);
            currentTexture = null;
        }

        byte[] bytes = Convert.FromBase64String(base64);
        currentTexture = new Texture2D(2, 2);
        currentTexture.LoadImage(bytes);
        Sprite sprite = Sprite.Create(currentTexture, new Rect(0, 0, currentTexture.width, currentTexture.height), new Vector2(0.5f, 0.5f), 100f);
        img.sprite = sprite;

        int index = currentIndex;
        string date = PlayerPrefs.GetString("ImageDate_" + index, " ");
        string location = PlayerPrefs.GetString("ImageLocation_" + index, " ");
        info_text.text = date + "\n" + location;
    }



    public void NextImage()
    {
        if (imagePaths.Count > 0 && currentIndex < imagePaths.Count - 1)
        {
            currentIndex++;
            LoadImage(imagePaths[currentIndex]);
        }
    }

    public void PreviousImage()
    {
        if (imagePaths.Count > 0 && currentIndex > 0)
        {
            currentIndex--;
            LoadImage(imagePaths[currentIndex]);
        }
    }
}
