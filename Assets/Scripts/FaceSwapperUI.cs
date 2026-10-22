using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public sealed class FaceSwapperUI : MonoBehaviour
{
    [SerializeField]
    public Dictionary<Button, GameObject> ButtonFaceMappings = new();

    public TMP_Text FaceDetectionStatus;

    public ARFaceManager faceManager;

    public ARSession session;

    private void Awake()
    {
        foreach (var (button, prefab) in ButtonFaceMappings)
        {
            button.onClick.AddListener(() =>
            {
                faceManager.facePrefab = prefab;
                session.Reset();
            });
        }
    }

    private void OnEnable()
    {
        faceManager.trackablesChanged.AddListener(OnFacesChanged);
    }

    private void OnDisable()
    {
        faceManager.trackablesChanged.RemoveListener(OnFacesChanged);
    }

    private void OnFacesChanged(ARTrackablesChangedEventArgs<ARFace> args)
    {
        var detected = false;

        foreach (var face in faceManager.trackables)
        {
            if (face.trackingState == TrackingState.Tracking)
            {
                detected = true;
                break;
            }
        }

        FaceDetectionStatus.text = detected ? "Face detected" : "No face detected";
        FaceDetectionStatus.color = detected ? Color.green : Color.red;
    }
}
