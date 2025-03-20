using System.Collections;
using UnityEngine;
using Zenject;

public class DogsBreedsPresenter : MonoBehaviour
{
    [Inject] DogsBreedsModel _dogsBreedsModel;
    [SerializeField] DogsBreedsView _dogsBreedsView;
    public void Init()
    {
        _dogsBreedsView.IChangedState+=HandleState;
        _dogsBreedsView.OnBreedSelected+=_dogsBreedsModel.FetchBreedInfo;
        _dogsBreedsModel.OnBreedsLoaded+=_dogsBreedsView.Refresh;
        _dogsBreedsModel.OnDogInfoLoaded+=_dogsBreedsView.ShowPopup;
    }
     
    void HandleState(bool f)
    {
        if(f) Enable();
        else Disable();
    }
    void Disable()
    {
        _dogsBreedsModel.CancelRequest();
    }
    void Enable()
    {
        _dogsBreedsModel.FetchBreeds();
    }
    //тут я не использовал коротину для вызова раз в несколько секунд, тк задаче этого не требует, поэтому я привязал ее го ивенту
    
}