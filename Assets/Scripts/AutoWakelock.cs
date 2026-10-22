using UnityEngine;

public class AutoWakelock : MonoBehaviour
{
    private int previousSleepTimeout;

    private void OnEnable()
    {
        previousSleepTimeout = Screen.sleepTimeout;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    private void OnDisable()
    {
        Screen.sleepTimeout = previousSleepTimeout;
    }
}
