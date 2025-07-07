using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AccessCamera : MonoBehaviour
{
    WebCamTexture webcam;
    public RawImage img;
    public Button captureButton;

    void Start()
    {
        webcam = new WebCamTexture();
        img.texture = webcam;
        img.material.mainTexture = webcam;
        webcam.Play();

        captureButton.onClick.AddListener(OnCaptureClick);
    }

    void Update()
    {
        if (webcam != null && webcam.width > 100)
        {
            img.texture = webcam;
            img.material.mainTexture = webcam;

            // Set the aspect ratio on the AspectRatioFitter
            AspectRatioFitter fitter = img.GetComponent<AspectRatioFitter>();
            if (fitter != null)
            {
                fitter.aspectRatio = (float)webcam.width / webcam.height;
            }

            // Rotation fix
            img.rectTransform.localEulerAngles = new Vector3(0, 0, -webcam.videoRotationAngle);

            // Flip horizontally if mirrored
            img.rectTransform.localScale = new Vector3(webcam.videoVerticallyMirrored ? -1 : 1, 1, 1);
        }

    }

    void OnCaptureClick()
    {
        StartCoroutine(CapturePhoto());
    }

    IEnumerator CapturePhoto()
    {
        yield return new WaitForEndOfFrame();

        Texture2D photo = new Texture2D(webcam.width, webcam.height);
        photo.SetPixels(webcam.GetPixels());
        photo.Apply();

        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string formattedDate = DateTime.Now.ToString("d.M.yyyy HH:mm");

        // Optionally encode to bytes if you ever need to
        //byte[] bytes = photo.EncodeToPNG();

        StartCoroutine(SavePhotoWithMetadata(photo, formattedDate));
    }

    IEnumerator SavePhotoWithMetadata(Texture2D photo, string formattedDate)
    {
        string location = "Unknown";

        if (Input.location.isEnabledByUser)
        {
            Input.location.Start();

            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            if (Input.location.status == LocationServiceStatus.Running)
            {
                float lat = Input.location.lastData.latitude;
                float lon = Input.location.lastData.longitude;

                bool done = false;
                StartCoroutine(ReverseGeocode(lat, lon, (result) =>
                {
                    location = result;
                    done = true;
                }));

                while (!done)
                    yield return null;
            }

            Input.location.Stop();
        }

        int imageCount = PlayerPrefs.GetInt("ImageCount", 0);

        // Save image to memory (as Base64 string)
        string base64 = Convert.ToBase64String(photo.EncodeToPNG());
        PlayerPrefs.SetString("ImageBase64_" + imageCount, base64);

        PlayerPrefs.SetString("ImageDate_" + imageCount, formattedDate);
        PlayerPrefs.SetString("ImageLocation_" + imageCount, location);
        PlayerPrefs.SetInt("ImageCount", imageCount + 1);
        PlayerPrefs.Save();

        Debug.Log("Photo metadata saved. Returning to main scene.");
        UnityEngine.SceneManagement.SceneManager.LoadScene("HomeScene"); 
    }

    IEnumerator ReverseGeocode(float lat, float lon, Action<string> callback)
    {
        string url = $"https://nominatim.openstreetmap.org/reverse?lat={lat}&lon={lon}&format=json";

        using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("User-Agent", "UnityApp");

            yield return www.SendWebRequest();

            if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                callback("Unknown Location");
            }
            else
            {
                try
                {
                    string json = www.downloadHandler.text;
                    var data = JsonUtility.FromJson<NominatimResult>(json);
                    string display = data.address.city ?? data.address.town ?? data.address.village ?? data.address.county ?? "Unknown";
                    if (!string.IsNullOrEmpty(data.address.country))
                        display += ", " + data.address.country;
                    callback(display);
                }
                catch
                {
                    callback("Unknown Location");
                }
            }
        }
    }

    [Serializable]
    private class NominatimResult
    {
        public Address address;
    }

    [Serializable]
    private class Address
    {
        public string city;
        public string town;
        public string village;
        public string county;
        public string country;
    }
}
