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
            string path = PlayerPrefs.GetString("ImagePath_" + i, "");
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                imagePaths.Add(path);
            }
        }
    }


    private void LoadImage(string path)
    {
        // Clean up the previous texture
        if (currentTexture != null)
        {
            Destroy(currentTexture);
            currentTexture = null;
        }

        byte[] bytes = File.ReadAllBytes(path);
        currentTexture = new Texture2D(2, 2);
        currentTexture.LoadImage(bytes);
        Sprite sprite = Sprite.Create(currentTexture, new Rect(0, 0, currentTexture.width, currentTexture.height), new Vector2(0.5f, 0.5f), 100f);
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
