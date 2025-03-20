using System.Collections;
using UnityEngine;
using Zenject;

public class WeatherPresenter :MonoBehaviour
{
    
    [Inject] WeatherModel _weatherModel;
    [SerializeField]WeatherView _weatherView;
    Coroutine _weatherUpdate;
    /*
    Презентер отвечат лишь за общение между сущностями, поэтому я связал ивенты модели и вью в методе инит.
    тк презентер и вью висят на одном объекте, я инициализирую их в MainController, когда запускается программа.
    */
    public void Init()
    {
        _weatherView.IChangedState+=HandleState;
        _weatherModel.OnWeatherRequestSuccess+=_weatherView.Refresh;
    }
     
    void HandleState(bool f)
    {
        if(f) Enable();
        else Disable();
    }
    void Disable()
    {
        if(_weatherUpdate!=null) StopCoroutine(_weatherUpdate);
        _weatherModel.CancelRequest();
    }
    void Enable()
    {
        _weatherUpdate= StartCoroutine(UpdateWeatherRoutine());
    }
    //тут реализован запрос каждые пять секунд.
    IEnumerator UpdateWeatherRoutine()
    {
        while (true)
        {
            _weatherModel.FetchWeather();
            yield return new WaitForSeconds(5);
        }
    }
}