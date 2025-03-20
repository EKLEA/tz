using UnityEngine;
using UnityEngine.UI;


public class MainController : MonoBehaviour
{
   
    public Button _lBT, _rBT;  
    public WeatherView weatherPage;
     public DogsBreedsView dogsBreedsPage;
    public WeatherPresenter weatherPresenter;
    public DogsBreedsPresenter dogsBreedsPresenter;
    
    void Start()
    {
       weatherPresenter.Init();
       dogsBreedsPresenter.Init();
       SetPage(true);
    }

    public void SetPage(bool f)
    {
        weatherPage.SetEnable(f);
        dogsBreedsPage.SetEnable(!f);
        _lBT.interactable=!f;
        _rBT.interactable=f;
    }
}
