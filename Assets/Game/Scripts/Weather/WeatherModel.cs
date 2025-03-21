using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Zenject;
using SimpleJSON;

public class WeatherModel 
{
    
    public event Action<WeatherArgs> OnWeatherRequestSuccess;
    [Inject] RequestQueue _requestQueue;
    UnityWebRequest _currentRequest;
    bool _isRequestInProgress = false;
    string imageURl;
    Sprite sprite;// закешировал чтобы постоянно не грузить
    
    public void FetchWeather()
    {
        if (_isRequestInProgress) return; 
        _requestQueue.AddRequest(SendWeatherRequest());
    }
    /*
        Тут я написал получение данных с апи. 
        Так же тут реализована загрузка картинки. Я ее закешировал, так в отличии от текстовых данных о погоде, картинка занимает место.
        Я реализовал логику, которая скачивает картинку если меняется ссылка на нее.
        работа с json выполняется с помошью сторонней библиотеки SimpleJSON
    */
    IEnumerator SendWeatherRequest()
    {
        _isRequestInProgress = true;
        using (_currentRequest = UnityWebRequest.Get("https://api.weather.gov/gridpoints/TOP/32,81/forecast"))
        {
            yield return _currentRequest.SendWebRequest();
            _isRequestInProgress = false;
            
           if (_currentRequest.result == UnityWebRequest.Result.Success)
            {
                string json = _currentRequest.downloadHandler.text;

                var jsonObject = JSON.Parse(json);
                
                if (jsonObject != null && jsonObject["properties"] != null && jsonObject["properties"]["periods"].Count > 0)
                {
                    var periodData = jsonObject["properties"]["periods"][0];
                    
                    
                    if(imageURl!= periodData["icon"])
                    {
                        
                        imageURl=periodData["icon"];
                        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageURl))
                        {
                            yield return request.SendWebRequest();

                            if (request.result == UnityWebRequest.Result.Success)
                            {
                                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                                sprite=ImageLoader.SpriteFromTexture(texture);
                            }
                            else
                            {
                                Debug.LogError("Ошибка загрузки: " + request.error);
                            }
                        }
                    }
                    
                    Period currentPeriod = new Period
                    {
                        temperature = periodData["temperature"].AsInt,
                        temperatureUnit = periodData["temperatureUnit"],
                        icon = sprite
                    };
                    OnWeatherRequestSuccess?.Invoke(new WeatherArgs(currentPeriod));
                }
                else
                {
                    Debug.LogError("Информация о погодей не найдена.");
                }
            }
            else
            {
                Debug.LogError($"Ошибка запроса: {_currentRequest.error}");
            }
        }
    }

    public void CancelRequest()
    {
        if (_currentRequest != null && _isRequestInProgress) 
        {
            _currentRequest.Abort();
            _isRequestInProgress = false;
        }
    }
}
// Общение между сущностями я реализовал через ивенты, чтобы модель не зависела от контроллера. А снизу я описал какие данные ивент должен передавать.
public class WeatherArgs: EventArgs
{
    public Period period;
    public WeatherArgs(Period p)
    {
        period=p;
    }
}

// тут представлены классы для удобного преобразования данных из json
internal class JsonDocument
{
    internal static JsonDocument Parse(string json)
    {
        throw new NotImplementedException();
    }
}
public class WeatherResponse
{
    public Properties properties;
}
public class Properties
{
    public Period[] periods;
}
public class Period
{
    public int temperature;
    public string temperatureUnit;
    public Sprite icon;
}
public class ImageLoader :MonoBehaviour
{
   public static  Sprite SpriteFromTexture(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}