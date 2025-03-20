using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    [SerializeField] RequestQueue requestQueue;
    public override void InstallBindings()
    {
        /*
            Забиндил RequestQueueт исползуется что в модели погоды, что в породах.
        */
        Container.Bind<RequestQueue>().FromInstance(requestQueue).AsSingle();
        /*
            Здесь я забиндил только модели, так не вижу смысла биндить презентеры. 
        Презентеров в проекте может быть много, и биндить каждый будет избыточно.
        
            Для примера можно взять модель погоды. Данные, получаемые через эту модель, могут быть использованы как для просто показа текущей погоды, 
        так и для реализации такой механики как синхронизации игровой погоды с реальной;
        
            Скрипты презентеров висят на геймобжектах вьюшек, но так как 
        view не зависит от контроллера, то при удалении ничего не будет.
        */
        Container.Bind<WeatherModel>().AsSingle();
        Container.Bind<DogsBreedsModel>().AsSingle();
        
        
        
    }
}
