using UnityEngine;

public class HapticManager : MonoBehaviour
{
    public static HapticManager Instance;

#if UNITY_ANDROID && !UNITY_EDITOR
    private static AndroidJavaObject vibrator;
    private static AndroidJavaClass vibrationEffectClass;
#endif

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeHaptics();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeHaptics()
    {
        // Ensures vibration permission is included
        if (Application.isEditor && false)
        {
            Handheld.Vibrate();
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            if (vibrator == null)
            {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    AndroidJavaObject currentActivity =
                        unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                    vibrator = currentActivity.Call<AndroidJavaObject>(
                        "getSystemService", "vibrator");
                }

                vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
                Debug.Log("Haptics initialized.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to initialize haptics: " + e.Message);
        }
#endif
    }

    public void Default()
    {
        TriggerHaptic(100, 255);
    }

    public void Light()
    {
        TriggerHaptic(20, 80);
    }

    public void Medium()
    {
        TriggerHaptic(40, 150);
    }

    public void Heavy()
    {
        TriggerHaptic(70, 255);
    }

    public void TriggerHaptic(long milliseconds, int amplitude)
    {
#if UNITY_ANDROID && !UNITY_EDITOR

        if (vibrator != null && vibrator.Call<bool>("hasVibrator"))
        {
            try
            {
                int sdkInt = new AndroidJavaClass("android.os.Build$VERSION")
                    .GetStatic<int>("SDK_INT");

                Debug.Log($"[Haptics] Triggering vibration ({milliseconds}ms, Amp {amplitude})");

                if (sdkInt >= 26 && amplitude < 255)
                {
                    AndroidJavaObject effect =
                        vibrationEffectClass.CallStatic<AndroidJavaObject>(
                            "createOneShot",
                            milliseconds,
                            amplitude);

                    vibrator.Call("vibrate", effect);
                }
                else
                {
                    vibrator.Call("vibrate", milliseconds);
                }

                Debug.Log("[Haptics] Vibration sent successfully.");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Haptic failed: " + e.Message);
                Handheld.Vibrate();
            }
        }

#else
        Debug.Log($"[Editor] Vibration {milliseconds}ms (Amp {amplitude})");
#endif
    }
}