using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class MarkerPrefabSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct MarkerPrefab
    {
        public string imageName;
        public GameObject prefab;
    }

    [SerializeField] private MarkerPrefab[] markerPrefabs;

    private ARTrackedImageManager trackedImageManager;
    private readonly Dictionary<string, GameObject> prefabsByName = new();
    private readonly Dictionary<TrackableId, GameObject> spawnedObjects = new();
    private readonly Dictionary<TrackableId, ARTrackedImage> trackedImages = new();
    private TrackableId activeImageId = TrackableId.invalidId;

    private void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
        foreach (var entry in markerPrefabs)
            prefabsByName[entry.imageName] = entry.prefab;
    }

    private void OnEnable()
    {
        trackedImageManager.trackablesChanged.AddListener(OnTrackablesChanged);
    }

    private void OnDisable()
    {
        trackedImageManager.trackablesChanged.RemoveListener(OnTrackablesChanged);
    }

    private void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach (var image in args.added)
        {
            if (!prefabsByName.TryGetValue(image.referenceImage.name, out var prefab))
                continue;

            var spawned = Instantiate(prefab, image.transform);
            spawned.SetActive(false);
            spawnedObjects[image.trackableId] = spawned;
            trackedImages[image.trackableId] = image;
        }

        foreach (var pair in args.removed)
        {
            if (spawnedObjects.Remove(pair.Key, out var spawned))
                Destroy(spawned);
            trackedImages.Remove(pair.Key);

            if (pair.Key == activeImageId)
                activeImageId = TrackableId.invalidId;
        }

        UpdateActiveObject();
    }

    private void UpdateActiveObject()
    {
        // Release the slot once the active marker is no longer tracked.
        if (activeImageId != TrackableId.invalidId &&
            trackedImages[activeImageId].trackingState != TrackingState.Tracking)
        {
            spawnedObjects[activeImageId].SetActive(false);
            activeImageId = TrackableId.invalidId;
        }

        if (activeImageId != TrackableId.invalidId)
            return;

        foreach (var pair in trackedImages)
        {
            if (pair.Value.trackingState != TrackingState.Tracking)
                continue;

            activeImageId = pair.Key;
            spawnedObjects[activeImageId].SetActive(true);
            break;
        }
    }
}