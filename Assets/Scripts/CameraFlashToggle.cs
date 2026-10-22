using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class CameraFlashToggle : MonoBehaviour
{
    [SerializeField] private ARCameraManager cameraManager;

    public void ToggleCameraFlash()
    {
        if (cameraManager == null || !cameraManager.DoesCurrentCameraSupportTorch())
            return;

        cameraManager.requestedCameraTorchMode =
            cameraManager.currentCameraTorchMode == XRCameraTorchMode.On
                ? XRCameraTorchMode.Off
                : XRCameraTorchMode.On;
    }
}
