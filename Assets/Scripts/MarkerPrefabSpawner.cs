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
            if (prefabsByName.TryGetValue(image.referenceImage.name, out var prefab))
                spawnedObjects[image.trackableId] = Instantiate(prefab, image.transform);
        }

        foreach (var image in args.updated)
        {
            if (spawnedObjects.TryGetValue(image.trackableId, out var spawned))
                spawned.SetActive(image.trackingState == TrackingState.Tracking);
        }

        foreach (var pair in args.removed)
        {
            if (spawnedObjects.Remove(pair.Key, out var spawned))
                Destroy(spawned);
        }
    }
}
