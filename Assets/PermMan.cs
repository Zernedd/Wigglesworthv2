using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class PermMan : MonoBehaviour
{
    
    private static PermMan _instance = null;
    public static PermMan Instance
    {
        get
        {
            if (_instance == null)
                _instance = (PermMan)FindObjectOfType(typeof(PermMan));
            return _instance;
        }
    }

#if PLATFORM_ANDROID
    private void Awake()
    {
        CheckForPermissions();
    }

    void CheckForPermissions()
    {
        if (!GetMicPermission())
        {
            SetMicPermission();
        }
    }

    bool GetMicPermission()
    {
        return Permission.HasUserAuthorizedPermission(Permission.Microphone);
    }

    public void SetMicPermission()
    {
        Permission.RequestUserPermission(Permission.Microphone);
    }
#endif
}
