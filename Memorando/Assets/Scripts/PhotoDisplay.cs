using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PhotoDisplay : MonoBehaviour
{
    public Image img;
    public Button nextButton;
    public Button prevButton;
    private List<string> imagePaths = new List<string>();
    private int currentIndex = 0;

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
            string path = PlayerPrefs.GetString("ImagePath_" + i, "");
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                imagePaths.Add(path);
            }
        }
    }

    private void LoadImage(string path)
    {
        byte[] bytes = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
        img.sprite = sprite;
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
