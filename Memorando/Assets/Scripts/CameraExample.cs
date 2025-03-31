using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class NfpCameraExample : MonoBehaviour
{
    /*public Image img;
    public Button nextButton;
    public Button prevButton;

    private List<string> imagePaths = new List<string>();
    private int currentIndex = 0;

    void Start()
    {
        LoadImagePaths();
        DisplayImage();
    }

    public void TakePhotoAndLoad()
    {
        if (NativeCamera.IsCameraBusy())
        {
            Debug.Log("Camera is busy");
            return;
        }

        NativeCamera.TakePicture((path) =>
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.Log("Operation cancelled");
                return;
            }

            Debug.Log("Photo saved at: " + path);

            // Generate a unique filename for the new image
            string newFilePath = Path.Combine(Application.persistentDataPath, "photo_" + System.DateTime.Now.Ticks + ".jpg");
            File.Copy(path, newFilePath, true);

            // Save path to PlayerPrefs
            int count = PlayerPrefs.GetInt("ImageCount", 0);
            PlayerPrefs.SetString("ImagePath_" + count, newFilePath);
            PlayerPrefs.SetInt("ImageCount", count + 1);
            PlayerPrefs.Save();

            // Update the list and display the new image
            imagePaths.Add(newFilePath);
            currentIndex = imagePaths.Count - 1;
            DisplayImage();

        }, maxSize: 1024);
    }

    void LoadImagePaths()
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

        if (imagePaths.Count > 0)
            currentIndex = 0; // Start with the latest image
    }

    void DisplayImage()
    {
        if (imagePaths.Count > 0 && currentIndex >= 0 && currentIndex < imagePaths.Count)
        {
            LoadImage(imagePaths[currentIndex]);
        }

        prevButton.interactable = currentIndex > 0;
        nextButton.interactable = currentIndex < imagePaths.Count - 1;
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
        if (currentIndex < imagePaths.Count - 1)
        {
            currentIndex++;
            DisplayImage();
        }
    }

    public void PreviousImage()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            DisplayImage();
        }
    }*/
}
