using UnityEngine;
using UnityEngine.UI;

public class VibrationTest : MonoBehaviour
{
    [SerializeField] private Button defaultVibrationButton;
    [SerializeField] private Button lightVibrationButton;
    [SerializeField] private Button mediumVibrationButton;
    [SerializeField] private Button heavyVibrationButton;

#if UNITY_ANDROID && !UNITY_EDITOR
    private static AndroidJavaObject vibrator;
    private static AndroidJavaClass vibrationEffectClass;
#endif

    private void Start()
    {
        InitializeHaptics();
    }

    private void OnEnable()
    {
        defaultVibrationButton.onClick.AddListener(() => TriggerHaptic(100, 255)); // Long/Heavy
        lightVibrationButton.onClick.AddListener(() => TriggerHaptic(20, 80));    // Light tick
        mediumVibrationButton.onClick.AddListener(() => TriggerHaptic(40, 150));  // Medium tap
        heavyVibrationButton.onClick.AddListener(() => TriggerHaptic(70, 255));   // Heavy knock
    }

    private void OnDisable()
    {
        defaultVibrationButton.onClick.RemoveAllListeners();
        lightVibrationButton.onClick.RemoveAllListeners();
        mediumVibrationButton.onClick.RemoveAllListeners();
        heavyVibrationButton.onClick.RemoveAllListeners();
    }

    private void InitializeHaptics()
    {
        // Trick the compiler to ensure the permission is baked in
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
                    AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                    vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                }
                vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
                Debug.Log("Haptics successfully initialized native Java objects.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Haptic initialization failed: " + e.Message);
        }
#endif
    }

    public void TriggerHaptic(long milliseconds, int amplitude)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (vibrator != null && vibrator.Call<bool>("hasVibrator"))
        {
            try
            {
                int sdkInt = new AndroidJavaClass("android.os.Build$VERSION").GetStatic<int>("SDK_INT");
                
                // If device is Android 8.0+ AND we are trying a light/medium vibe
                if (sdkInt >= 26 && amplitude < 255)
                {
                    AndroidJavaObject effect = vibrationEffectClass.CallStatic<AndroidJavaObject>(
                        "createOneShot", milliseconds, amplitude);
                    vibrator.Call("vibrate", effect);
                }
                else
                {
                    // FALLBACK: Heavy raw milliseconds call. Android cannot ignore this.
                    // This uses the old API which bypasses custom OS amplitude restrictions.
                    vibrator.Call("vibrate", milliseconds);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Haptic trigger failed, running safety fallback: " + e.Message);
                Handheld.Vibrate(); // Absolute emergency fallback
            }
        }
        else
        {
            Debug.LogWarning("Vibrator hardware reports as missing or null!");
        }
#else
        Debug.Log($"[Editor] Vibe: {milliseconds}ms, Amp: {amplitude}");
#endif
    }
}