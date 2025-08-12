using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

[Serializable]
public class PhotoMetadata
{
    public string filePath;
    public string date;
    public string location;
}
public class PhotoDisplay : MonoBehaviour
{
    public RawImage img;
    public Button nextButton;
    public Button prevButton;
    public TMP_Text info_text;

    private List<PhotoMetadata> photoList = new List<PhotoMetadata>();
    private int currentIndex = 0;
    private Texture2D currentTexture = null;
    private string photoDirectory => Path.Combine(Application.persistentDataPath, "Photos");
    private string metadataFile => Path.Combine(photoDirectory, "metadata.json");

    void Start()
    {
        LoadMetadata();
        if (photoList.Count > 0)
        {
            currentIndex = 0; // Start from the first image
            LoadImage(photoList[currentIndex]);
        }
    }

    private void LoadMetadata()
    {
        photoList.Clear();
        if (File.Exists(metadataFile))
        {
            string json = File.ReadAllText(metadataFile);
            photoList = JsonUtility.FromJson<PhotoListWrapper>(json).photos;
        }
    }

    [Serializable]
    private class PhotoListWrapper
    {
        public List<PhotoMetadata> photos = new();
    }

    private void LoadImage(PhotoMetadata metadata)
    {
        if (currentTexture != null)
        {
            Destroy(currentTexture);
            currentTexture = null;
        }

        string path = metadata.filePath;
        if (!File.Exists(path))
        {
            Debug.LogWarning("Image file not found: " + path);
            return;
        }

        byte[] bytes = File.ReadAllBytes(path);
        currentTexture = new Texture2D(2, 2);
        currentTexture.LoadImage(bytes);
        img.texture = currentTexture;

        // Apply aspect ratio
        AspectRatioFitter fitter = img.GetComponent<AspectRatioFitter>();
        if (fitter != null)
        {
            fitter.aspectRatio = (float)currentTexture.width / currentTexture.height;
        }

        // Reset and then set desired rotation/scale
        img.rectTransform.localEulerAngles = Vector3.zero;
        img.rectTransform.localScale = Vector3.one;

        // If your images need -90° to appear upright:
        img.rectTransform.localEulerAngles = new Vector3(0, 0, -90f);

        info_text.text = $"{metadata.date}\n{metadata.location}";
    }





    public void NextImage()
    {
        if (photoList.Count > 0 && currentIndex < photoList.Count - 1)
        {
            currentIndex++;
            LoadImage(photoList[currentIndex]);
        }
    }

    public void PreviousImage()
    {
        if (photoList.Count > 0 && currentIndex > 0)
        {
            currentIndex--;
            LoadImage(photoList[currentIndex]);
        }
    }

}
