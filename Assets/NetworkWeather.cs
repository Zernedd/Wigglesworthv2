using UnityEngine;
using Photon.Pun;
using System.Collections;

public class NetworkWeather : MonoBehaviourPunCallbacks, IPunObservable
{
    public enum WeatherType { Sunny, Rainy, Thunderstorm }

    [Header("Skyboxes")]
    public Cubemap sunnyDaySkybox;
    public Cubemap rainyDaySkybox;
    public Cubemap thunderstormDaySkybox;
    public Cubemap sunnyNightSkybox;
    public Cubemap rainyNightSkybox;
    public Cubemap thunderstormNightSkybox;

    [Header("Effects")]
    public GameObject sunnyEffects;
    public GameObject rainyEffects;
    public GameObject thunderstormEffects;
    public GameObject[] dayObjects;
    public GameObject[] nightObjects;
    public GameObject sunGameObject; // Reference to your sun GameObject

    [Header("Settings")]
    public float weatherChangeInterval = 120f;
    public float dayLengthInMinutes = 10f;

    [Header("References")]
    public Light sunLight; // Keep this for any light-specific logic
    public Gradient sunColor;
    public AnimationCurve sunIntensity;

    private WeatherType currentWeather;
    private float weatherTimer;
    private float dayCycleProgress;
    private Material skyboxMaterial;
    private Vector3 initialSunRotation = new Vector3(56.096f, -35.182f, -6.473f);

    void Start()
    {
        skyboxMaterial = new Material(Shader.Find("Skybox/Cubemap"));
        RenderSettings.skybox = skyboxMaterial;

        if (sunLight != null)
        {
            sunLight.transform.localEulerAngles = initialSunRotation;
        }

        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(WeatherUpdateLoop());
            }
        }
        else
        {
            StartCoroutine(WeatherUpdateLoop());
        }
    }

    IEnumerator WeatherUpdateLoop()
    {
        while (true)
        {
            weatherTimer += Time.deltaTime;
            dayCycleProgress += Time.deltaTime / (dayLengthInMinutes * 60f);
            if (dayCycleProgress >= 1f) dayCycleProgress = 0f;

            if (weatherTimer >= weatherChangeInterval)
            {
                weatherTimer = 0f;
                WeatherType newWeather = (WeatherType)Random.Range(0, 3);

                if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
                {
                    photonView.RPC("SetWeather", RpcTarget.All, (int)newWeather);
                }
                else
                {
                    SetWeather((int)newWeather);
                }
            }

            UpdateDayNightCycle();
            yield return null;
        }
    }

    [PunRPC]
    void SetWeather(int weatherIndex)
    {
        currentWeather = (WeatherType)weatherIndex;
        ApplyWeather();
    }

    void ApplyWeather()
    {
        // Toggle weather effects
        if (sunnyEffects) sunnyEffects.SetActive(currentWeather == WeatherType.Sunny);
        if (rainyEffects) rainyEffects.SetActive(currentWeather == WeatherType.Rainy);
        if (thunderstormEffects) thunderstormEffects.SetActive(currentWeather == WeatherType.Thunderstorm);

        UpdateSunVisibility();
        UpdateSkybox();
    }

    void UpdateDayNightCycle()
    {
        bool isDay = dayCycleProgress < 0.5f;

        // Update sun rotation and properties (but don't disable the Light component)
        if (sunLight != null)
        {
            float sunAngle = dayCycleProgress * 360f;
            sunLight.transform.localEulerAngles = new Vector3(
                initialSunRotation.x + sunAngle,
                initialSunRotation.y,
                initialSunRotation.z
            );

            sunLight.intensity = sunIntensity.Evaluate(dayCycleProgress);
            sunLight.color = sunColor.Evaluate(dayCycleProgress);
        }

        // Update objects
        foreach (var obj in dayObjects) if (obj != null) obj.SetActive(isDay);
        foreach (var obj in nightObjects) if (obj != null) obj.SetActive(!isDay);

        UpdateSunVisibility();
        UpdateSkybox();
    }

    void UpdateSunVisibility()
    {
        if (sunGameObject != null)
        {
            // Only disable the GameObject during night or bad weather
            bool shouldBeVisible = (currentWeather == WeatherType.Sunny) && (dayCycleProgress < 0.5f);
            sunGameObject.SetActive(shouldBeVisible);
        }
    }

    void UpdateSkybox()
    {
        bool isDay = dayCycleProgress < 0.5f;
        Cubemap skybox = isDay ? GetDaySkybox() : GetNightSkybox();
        if (skybox != null)
        {
            skyboxMaterial.SetTexture("_Tex", skybox);
            DynamicGI.UpdateEnvironment();
        }
    }

    Cubemap GetDaySkybox()
    {
        switch (currentWeather)
        {
            case WeatherType.Sunny: return sunnyDaySkybox;
            case WeatherType.Rainy: return rainyDaySkybox;
            case WeatherType.Thunderstorm: return thunderstormDaySkybox;
            default: return sunnyDaySkybox;
        }
    }

    Cubemap GetNightSkybox()
    {
        switch (currentWeather)
        {
            case WeatherType.Sunny: return sunnyNightSkybox;
            case WeatherType.Rainy: return rainyNightSkybox;
            case WeatherType.Thunderstorm: return thunderstormNightSkybox;
            default: return sunnyNightSkybox;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext((int)currentWeather);
            stream.SendNext(dayCycleProgress);
        }
        else
        {
            currentWeather = (WeatherType)stream.ReceiveNext();
            dayCycleProgress = (float)stream.ReceiveNext();
            ApplyWeather();
            UpdateDayNightCycle();
        }
    }
}