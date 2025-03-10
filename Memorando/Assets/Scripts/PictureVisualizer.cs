using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PictureVisualizer : MonoBehaviour
{
    private string photoPath;
    public RawImage PictureFrame;
    public TMP_Text debugLabel;

    void Start()
    {
        Invoke("LoadLastImageFromFile", 1.0f); // Delayed load
    }

    public void buttonToOpenCamera()
    {
        OpenCamera();
    }

    private void LoadLastImageFromFile()
    {
        photoPath = PlayerPrefs.GetString("LastPhotoPath", ""); // Ensure correct path

        debugLabel.text = photoPath;

        if (string.IsNullOrEmpty(photoPath) || !File.Exists(photoPath))
        {
            Debug.LogError("No photo found or file does not exist: " + photoPath);
            PictureFrame.texture = null;
            debugLabel.text += " - which does not exist";
            return;
        }

        try
        {
            byte[] imageBytes = File.ReadAllBytes(photoPath);
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(imageBytes))
            {
                texture.Apply();

                if (PictureFrame.texture != null)
                {
                    Destroy(PictureFrame.texture); // Free memory
                }

                PictureFrame.texture = texture;
                Debug.Log("Image loaded from: " + photoPath);
            }
            else
            {
                Debug.LogError("Failed to load image data.");
                PictureFrame.texture = null;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error loading image: " + e.Message);
            PictureFrame.texture = null;
        }
    }

    private void OpenCamera()
    {
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
            AndroidJavaClass intentClass = new AndroidJavaClass("android.provider.MediaStore");

            intent.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_IMAGE_CAPTURE"));

            // Generate a new filename based on the current date and time
            photoPath = Path.Combine(Application.persistentDataPath, "memorando_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".jpg");

            // Save the path to PlayerPrefs
            PlayerPrefs.SetString("LastPhotoPath", photoPath);
            PlayerPrefs.Save(); // Ensure it's saved immediately

            // Don't force a save path, let the system handle it
            currentActivity.Call("startActivityForResult", intent, 1);

            Debug.Log("Camera opened. Waiting for image...");
            Invoke("RetrieveLatestPhoto", 3.0f); // Wait a few seconds, then check for new photos
        }
        catch (Exception e)
        {
            Debug.LogError("Error opening camera: " + e.Message);
        }
    }

    // This function retrieves the most recent photo from the gallery
    private void RetrieveLatestPhoto()
    {
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaObject resolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
            AndroidJavaObject cursor = resolver.Call<AndroidJavaObject>(
                "query",
                new AndroidJavaObject("android.provider.MediaStore$Images$Media", "EXTERNAL_CONTENT_URI"),
                null, null, null, "date_added DESC"
            );

            if (cursor.Call<bool>("moveToFirst"))
            {
                int columnIndex = cursor.Call<int>("getColumnIndex", "_data");
                photoPath = cursor.Call<string>("getString", columnIndex);

                PlayerPrefs.SetString("LastPhotoPath", photoPath);
                PlayerPrefs.Save();
                Debug.Log("Latest photo retrieved: " + photoPath);

                // Load the photo now that we have the correct path
                LoadLastImageFromFile();
            }

            cursor.Call("close");
        }
        catch (Exception e)
        {
            Debug.LogError("Error retrieving latest photo: " + e.Message);
        }
    }
}
