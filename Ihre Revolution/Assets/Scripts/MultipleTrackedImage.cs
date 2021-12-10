using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

public class MultipleTrackedImage : MonoBehaviour

{
    public float timer;
    public ARTrackedImageManager trackedImageManager;
    public List<GameObject> objectList = new List<GameObject>();
    private Dictionary<string, GameObject> prefabDic = new Dictionary<string, GameObject>();
    private List<ARTrackedImage> trackedImg = new List<ARTrackedImage>();
    private List<float> trackedTimer = new List<float>();

    void Awake()
    {
        foreach (GameObject obj in objectList)
        {
            string tName = obj.name;
            prefabDic.Add(tName, obj);
        }
    }
    void Update()
    {
        if (trackedImg.Count > 0)
        {
            List<ARTrackedImage> tNumList = new List<ARTrackedImage>();
            for (var i = 0; i < trackedImg.Count; i++)
            {
                if (trackedImg[i].trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Limited)
                {
                    if (trackedTimer[i] > timer)
                    {
                        string name = trackedImg[i].referenceImage.name;
                        GameObject tObj = prefabDic[name];
                        tObj.SetActive(false);
                        tNumList.Add(trackedImg[i]);
                    }
                    else
                    {
                        trackedTimer[i] += Time.deltaTime;
                    }
                }
            }
            if (tNumList.Count > 0)
            {
                for (var i = 0; i < tNumList.Count; i++)
                {
                    int num = trackedImg.IndexOf(tNumList[i]);
                    trackedImg.Remove(trackedImg[num]);
                    trackedTimer.Remove(trackedTimer[num]);
                }
            }
        }
    }
    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += ImageChanged;
    }
    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= ImageChanged;
    }
    void ImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            if (!trackedImg.Contains(trackedImage))
            {
                trackedImg.Add(trackedImage);
                trackedTimer.Add(0);
            }
        }
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            if (!trackedImg.Contains(trackedImage))
            {
                trackedImg.Add(trackedImage);
                trackedTimer.Add(0);
            }
            else
            {
                int num = trackedImg.IndexOf(trackedImage);
                trackedTimer[num] = 0;
            }
            UpdateImage(trackedImage);
        }
    }
    void UpdateImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;
        GameObject tObj = prefabDic[name];
        tObj.transform.position = trackedImage.transform.position;
        tObj.transform.rotation = trackedImage.transform.rotation;
        tObj.SetActive(true);
    }
}