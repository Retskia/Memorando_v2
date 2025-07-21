using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class PhotoDisplay : MonoBehaviour
{
    public RawImage img;
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
        img.texture = currentTexture;

        // Apply aspect ratio
        AspectRatioFitter fitter = img.GetComponent<AspectRatioFitter>();
        if (fitter != null)
        {
            fitter.aspectRatio = (float)currentTexture.width / currentTexture.height;
        }

        // Apply rotation and flip
        img.rectTransform.localEulerAngles = Vector3.zero; // Reset rotation
        img.rectTransform.localScale = Vector3.one;        // Reset scale

        // Apply the same rotation and flip as the camera view
        img.rectTransform.localEulerAngles = new Vector3(0, 0, -90);
        img.rectTransform.localScale = new Vector3(1, 1, 1);
        // Camera was mirrored vertically

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

    /*private Texture2D FlipTextureVertically(Texture2D original)
    {
        Texture2D flipped = new Texture2D(original.width, original.height);
        for (int y = 0; y < original.height; y++)
        {
            flipped.SetPixels(0, y, original.width, 1, original.GetPixels(0, original.height - 1 - y, original.width, 1));
        }
        flipped.Apply();
        return flipped;
    }*/

}
