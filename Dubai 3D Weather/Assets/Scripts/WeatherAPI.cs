using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class WeatherAPI : MonoBehaviour
{
    //public const string SERVER_API_URL_FORMAT = "http://www.api.openweathermap.org/data/2.5/weather?q=London&appid=1bd65cb959ab6e20f614c0ca46711fac";

    private const string API_KEY = "1bd65cb959ab6e20f614c0ca46711fac";
    private const string CityId = "292223"; //Dubai

    private const float API_CHECK_MAXTIME = 10.0f; //10 seconds
    private float apiCheckCountdown = API_CHECK_MAXTIME;

    [Serializable]
    public class WeatherMain
    {
        public int id;
        public string main;
    }
    [Serializable]
    public class WeatherInfo
    {
        public int id;
        public string name;
        public List<WeatherMain> weather;
    }

    IEnumerator GetWeather(Action<WeatherInfo> onSuccess)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(String.Format("http://api.openweathermap.org/data/2.5/weather?id={0}&APPID={1}", CityId, API_KEY)))
        {
            yield return req.Send();
            while (!req.isDone)
                yield return null;
            byte[] result = req.downloadHandler.data;
            string weatherJSON = System.Text.Encoding.Default.GetString(result);
            Debug.Log(weatherJSON);
            WeatherInfo info = JsonUtility.FromJson<WeatherInfo>(weatherJSON);
            onSuccess(info);
        }
    }

    void Start()
    {
        StartCoroutine(GetWeather(CheckSnowStatus));
    }

    void Update()
    {
        apiCheckCountdown -= Time.deltaTime;
        if (apiCheckCountdown <= 0)
        {
            apiCheckCountdown = API_CHECK_MAXTIME;
            StartCoroutine(GetWeather(CheckSnowStatus));
        }
    }


    public void CheckSnowStatus(WeatherInfo weatherObj)
    {
        bool snowing = weatherObj.weather[0].main.Equals("Snow");
        if (snowing)
        {
            // SnowSystem.SetActive(true);
            Debug.Log("NIEVE");
        }
        else
        {
            // SnowSystem.SetActive(false);
            Debug.Log("NO NIEVE");
        }
    }
}
