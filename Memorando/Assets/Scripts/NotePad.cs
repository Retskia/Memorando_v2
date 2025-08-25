using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class NotePad : MonoBehaviour
{
    public GameObject Memopad;
    public TMP_InputField noteInput;

    void Start()
    {
        if (PlayerPrefs.GetInt("ShowMemopad", 0) == 1)
        {
            Memopad.SetActive(true);
            PlayerPrefs.SetInt("ShowMemopad", 0); // reset for next time
        }
    }
    public void HideMemopad()
    {
        Memopad.SetActive(false);
    }

    [System.Serializable]
    public class PhotoMetadata
    {
        public string filePath;
        public string date;
        public string location;
        public string note;
    }

    [System.Serializable]
    private class PhotoListWrapper
    {
        public List<PhotoMetadata> photos = new();
    }


    public void SaveNote()
    {
        string photoDirectory = Path.Combine(Application.persistentDataPath, "Photos");
        string metadataFile = Path.Combine(photoDirectory, "metadata.json");

        if (!File.Exists(metadataFile)) return;

        string json = File.ReadAllText(metadataFile);
        PhotoListWrapper wrapper = JsonUtility.FromJson<PhotoListWrapper>(json);

        string lastPhotoPath = PlayerPrefs.GetString("LastPhotoPath", "");

        foreach (var photo in wrapper.photos)
        {
            if (photo.filePath == lastPhotoPath)
            {
                photo.note = noteInput.text;
                break;
            }
        }

        string newJson = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(metadataFile, newJson);
        Memopad.SetActive(false);

        Debug.Log("Note saved to photo!");
    }
}
