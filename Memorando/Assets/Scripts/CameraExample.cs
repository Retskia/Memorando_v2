using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class NfpCameraExample : MonoBehaviour
{
    public Image img;
    private string savedPhotoPath;

    void Start()
    {
        LoadSavedImage();
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

            // Move the image to a persistent storage location
            string newFilePath = Path.Combine(Application.persistentDataPath, "saved_photo.jpg");
            File.Copy(path, newFilePath, true);

            // Save the path for future use
            PlayerPrefs.SetString("SavedPhotoPath", newFilePath);
            PlayerPrefs.Save();

            // Load and display the image
            LoadImage(newFilePath);

        }, maxSize: 1024);
    }

    private void LoadSavedImage()
    {
        string path = PlayerPrefs.GetString("SavedPhotoPath", "");

        if (!string.IsNullOrEmpty(path) && File.Exists(path))
        {
            Debug.Log("Loading saved photo: " + path);
            LoadImage(path);
        }
        else
        {
            Debug.Log("No saved photo found");
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
}
