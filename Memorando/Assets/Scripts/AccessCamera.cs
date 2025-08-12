using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // Required for TMP_Text
using UnityEngine.Networking;
using System.Collections.Generic;
using System.IO;

public class AccessCamera : MonoBehaviour
{
    WebCamTexture webcam;
    public RawImage img;
    public Button captureButton;
    public TMP_Text countdown; // Assign this in the Unity Editor

    private float countdownTime = 300f; // 5 minutes in seconds
    private bool webcamInitialized = false;

    void Start()
    {
        SetupWebcam();

        // Get fire time from PlayerPrefs to calculate remaining countdown
        if (PlayerPrefs.HasKey("CameraCountdownFireTime"))
        {
            string fireTimeStr = PlayerPrefs.GetString("CameraCountdownFireTime");
            if (DateTime.TryParseExact(fireTimeStr, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime fireTime))
            {
                TimeSpan elapsed = DateTime.Now - fireTime;
                float secondsElapsed = (float)elapsed.TotalSeconds;
                countdownTime = Mathf.Clamp(300f - secondsElapsed, 0f, 300f);
            }
        }

        captureButton.onClick.AddListener(OnCaptureClick);

        StartCoroutine(CountdownCoroutine());
    }

    void SetupWebcam()
    {
        webcam = new WebCamTexture();
        img.texture = webcam;
        img.material.mainTexture = webcam;
        webcam.Play();
    }

    void Update()
    {
        if (webcam != null && webcam.width > 100 && !webcamInitialized)
        {
            img.texture = webcam;
            img.material.mainTexture = webcam;

            // Match aspect ratio
            AspectRatioFitter fitter = img.GetComponent<AspectRatioFitter>();
            if (fitter != null)
                fitter.aspectRatio = (float)webcam.width / webcam.height;

            // Fix rotation
            img.rectTransform.localEulerAngles = new Vector3(0, 0, -webcam.videoRotationAngle);

            // Flip if mirrored
            img.rectTransform.localScale = new Vector3(webcam.videoVerticallyMirrored ? -1 : 1, 1, 1);

            webcamInitialized = true;
        }
    }

    private void OnCaptureClick()
    {
        StartCoroutine(CapturePhoto());
    }

    private IEnumerator CountdownCoroutine()
    {
        while (countdownTime > 0)
        {
            TimeSpan time = TimeSpan.FromSeconds(countdownTime);
            countdown.text = time.ToString(@"m\:ss");
            yield return new WaitForSeconds(1f);
            countdownTime -= 1f;
        }

        countdown.text = "Time's up!";
        yield return new WaitForSeconds(1f);

        // Optional: clear saved fire time
        PlayerPrefs.DeleteKey("CameraCountdownFireTime");

        SceneManager.LoadScene("HomeScene");
    }

    IEnumerator CapturePhoto()
    {
        yield return new WaitForEndOfFrame();

        Texture2D photo = new Texture2D(webcam.width, webcam.height);
        photo.SetPixels(webcam.GetPixels());
        photo.Apply();

        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string formattedDate = DateTime.Now.ToString("d.M.yyyy HH:mm");

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

        string photoDirectory = Path.Combine(Application.persistentDataPath, "Photos");
        if (!Directory.Exists(photoDirectory))
            Directory.CreateDirectory(photoDirectory);

        string filename = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string filePath = Path.Combine(photoDirectory, filename);
        File.WriteAllBytes(filePath, photo.EncodeToPNG());

        // Save metadata
        string metadataFile = Path.Combine(photoDirectory, "metadata.json");
        List<PhotoMetadata> photoList = new List<PhotoMetadata>();

        if (File.Exists(metadataFile))
        {
            string existingJson = File.ReadAllText(metadataFile);
            try
            {
                photoList = JsonUtility.FromJson<PhotoListWrapper>(existingJson).photos;
            }
            catch { }
        }

        PhotoMetadata newPhoto = new PhotoMetadata
        {
            filePath = filePath,
            date = formattedDate,
            location = location
        };
        photoList.Add(newPhoto);

        string jsonToSave = JsonUtility.ToJson(new PhotoListWrapper { photos = photoList }, true);
        File.WriteAllText(metadataFile, jsonToSave);

        PlayerPrefs.DeleteKey("CameraCountdownFireTime");
        SceneManager.LoadScene("HomeScene");
    }

    IEnumerator ReverseGeocode(float lat, float lon, Action<string> callback)
    {
        string url = $"https://nominatim.openstreetmap.org/reverse?lat={lat}&lon={lon}&format=json";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("User-Agent", "UnityApp");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
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

    [Serializable]
    public class PhotoMetadata
    {
        public string filePath;
        public string date;
        public string location;
    }

    [Serializable]
    private class PhotoListWrapper
    {
        public List<PhotoMetadata> photos = new();
    }
}
