using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class NfpCameraExample : MonoBehaviour
{
    public Image img;

    void Start()
    {
        RequestPermissionAsynchronously();
    }

    private async void RequestPermissionAsynchronously()
    {
        NativeFilePicker.Permission permission = await NativeFilePicker.RequestPermissionAsync(false);
        Debug.Log("Permission result: " + permission);
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

            // Load the image into a Texture2D
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);

            // Convert to Sprite
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
            img.sprite = sprite;

        }, maxSize: 1024); // Set max resolution
    }
}
