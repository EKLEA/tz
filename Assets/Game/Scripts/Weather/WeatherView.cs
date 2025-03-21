using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System.ComponentModel;
using Zenject;

public class WeatherView: MonoBehaviour
{
    [SerializeField] Image weatherIcon;
    [SerializeField] TextMeshProUGUI weatherText;
    public event Action<bool> IChangedState; //ивент для контролера, чтобы он сотанавливал работу модели
    
    
    public void Refresh(WeatherArgs t)
    {
        weatherText.text=string.Format("Сегодня - {0}{1}",t.period.temperature,t.period.temperatureUnit);
        weatherIcon.sprite=t.period.icon;
    }

    public void SetEnable(bool flag)
    {
        gameObject.SetActive(flag);
        IChangedState?.Invoke(flag);
    }
}
