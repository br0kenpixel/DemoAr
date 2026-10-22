using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PreferHighRefershRate : MonoBehaviour
{
    [SerializeField] ARCameraManager cameraManager;
    bool m_Done;

    void OnEnable() => cameraManager.frameReceived += OnFrame;
    void OnDisable() => cameraManager.frameReceived -= OnFrame;

    void OnFrame(ARCameraFrameEventArgs args)
    {
        if (m_Done) return;
        m_Done = true; // configurations are only available once the camera is running

        using var configs = cameraManager.GetConfigurations(Allocator.Temp);
        foreach (var config in configs)
        {
            Debug.Log($"Camera config: {config}");
            if (config.framerate == 60)
            {
                cameraManager.currentConfiguration = config;
                Debug.Log("Enabled high refresh rate camera");
                return;
            }
        }
        Debug.Log("No 60 fps camera configuration on this device/camera.");
    }
}
