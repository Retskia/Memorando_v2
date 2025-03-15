using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PhotoDisplay : MonoBehaviour
{
    public Image img;

    void Start()
    {
        LoadSavedImage();
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