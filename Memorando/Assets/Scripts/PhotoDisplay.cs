using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using NativeGalleryNamespace;

[Serializable]
public class PhotoMetadata
{
    public string filePath;
    public string date;
    public string location;
    public string note;
}
public class PhotoDisplay : MonoBehaviour
{
    public RawImage img;
    public Button nextButton;
    public Button prevButton;
    public GameObject Panel;
    public TMP_Text info_text;
    public TMP_Text noteText;
    public Button noteButton;
    public Sprite noteEmptySprite;
    public Sprite noteExistsSprite;
    private Image buttonImage;

    private List<PhotoMetadata> photoList = new List<PhotoMetadata>();
    private int currentIndex = 0;
    private Texture2D currentTexture = null;
    private string photoDirectory => Path.Combine(Application.persistentDataPath, "Photos");
    private string metadataFile => Path.Combine(photoDirectory, "metadata.json");

    void Start()
    {
        buttonImage = noteButton.GetComponent<Image>();
        LoadMetadata();
        if (photoList.Count > 0)
        {
            currentIndex = PlayerPrefs.GetInt("LastViewedPhotoIndex", 0);
            currentIndex = Mathf.Clamp(currentIndex, 0, photoList.Count - 1);
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
        UpdateButtonIcon(metadata);
    }

    public void SavePhotoToGallery()
    {
        if (photoList.Count == 0) return;

        PhotoMetadata metadata = photoList[currentIndex];

        if (!File.Exists(metadata.filePath))
        {
            Debug.LogWarning("Photo file not found: " + metadata.filePath);
            return;
        }

        byte[] bytes = File.ReadAllBytes(metadata.filePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);

        // Async version
        Texture2D rotated = RotateTexture(texture, 90); // rotate 90° clockwise
        NativeGallery.SaveImageToGallery(rotated, "Memorando", "photo_{0}.png", (success, path) =>
        {
            Destroy(rotated);
            Destroy(texture);
        });
    }

    Texture2D RotateTexture(Texture2D tex, float angle)
    {
        Color32[] pix = tex.GetPixels32();
        Color32[] rotatedPix = new Color32[pix.Length];
        int w = tex.width;
        int h = tex.height;

        for (int j = 0; j < h; ++j)
        {
            for (int i = 0; i < w; ++i)
            {
                int newI = j;
                int newJ = w - 1 - i;
                rotatedPix[newJ * h + newI] = pix[j * w + i];
            }
        }

        Texture2D rotatedTex = new Texture2D(h, w);
        rotatedTex.SetPixels32(rotatedPix);
        rotatedTex.Apply();
        return rotatedTex;
    }


    private void UpdateButtonIcon(PhotoMetadata metadata)
    {
        if (!string.IsNullOrEmpty(metadata.note))
            buttonImage.sprite = noteExistsSprite;
        else
            buttonImage.sprite = noteEmptySprite;
    }


    public void NextImage()
    {
        if (photoList.Count > 0 && currentIndex < photoList.Count - 1)
        {
            currentIndex++;
            LoadImage(photoList[currentIndex]);
            PlayerPrefs.SetInt("LastViewedPhotoIndex", currentIndex);
            PlayerPrefs.Save();
        }
    }

    public void PreviousImage()
    {
        if (photoList.Count > 0 && currentIndex > 0)
        {
            currentIndex--;
            LoadImage(photoList[currentIndex]);
            PlayerPrefs.SetInt("LastViewedPhotoIndex", currentIndex);
            PlayerPrefs.Save();
        }
    }

    public void OpenNotes()
    {
        Panel.SetActive(true);
        if (photoList.Count > 0)
        {
            noteText.text = photoList[currentIndex].note;
        }
    }

    public void HideNotes()
    {
        Panel.SetActive(false);
    }

}
