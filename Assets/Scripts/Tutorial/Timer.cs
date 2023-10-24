using System;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI display;
    [SerializeField] UnityEngine.UI.Image background;

    float timeStarted;
    TimeSpan
        timeElapsed,
        bestTime = TimeSpan.MaxValue,
        lastTime;


    void Awake()
    {
        SetVisible(false);
    }

    void Update()
    {
        timeElapsed = TimeSpan.FromSeconds(Time.time - timeStarted);
        display.text = string.Format("{0:D1}:{1:D2}", timeElapsed.Minutes, timeElapsed.Seconds);
    }

    public void StartTimer()
    {
        timeStarted = Time.time;
        SetVisible(true);
    }

    public void StopTimer(bool record = true)
    {
        if (record)
        {
            if (timeElapsed < bestTime)
            {
                bestTime = timeElapsed;
            }
            lastTime = timeElapsed;
        }
        SetVisible(false);
    }

    void SetVisible(bool isVisible)
    {
        display.enabled = isVisible;
        background.enabled = isVisible;
    }

    string ConvertTimeSpanToString(TimeSpan timeSpan)
    {
        string result =
            timeSpan.Minutes > 0 ?
                timeSpan.Minutes +
                    " minute" +
                    (timeSpan.Minutes == 1 ? "" : "s") + " and " :
                "";
        result +=
            timeSpan.Seconds + " second" +
            (timeSpan.Seconds == 1 ? "" : "s");

        Debug.Log(timeSpan);

        return result;
    }

    public string GetLastTime() => ConvertTimeSpanToString(lastTime);
    public string GetBestTime() => ConvertTimeSpanToString(bestTime);


}
