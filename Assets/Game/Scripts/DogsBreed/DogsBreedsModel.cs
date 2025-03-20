using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class DogsBreedsModel
{
    // так как модуль пород собак похож на модуль погоды, я буду описывать лишь то, чем он отличается.
    public event Action<BreedsArgs> OnBreedsLoaded;
    public event Action<BreedInfoArgs> OnDogInfoLoaded;

    [Inject] RequestQueue _requestQueue;
    UnityWebRequest _currentRequest;
    bool _isRequestInProgress = false;
    public void FetchBreeds()
    {
        if (_isRequestInProgress) return;
        _requestQueue.AddRequest(SendBreedsRequest());
    }

    IEnumerator SendBreedsRequest()
    {
        _isRequestInProgress = true;
        using (_currentRequest = UnityWebRequest.Get("https://dogapi.dog/api/v2/breeds"))
        {
            yield return _currentRequest.SendWebRequest();
            _isRequestInProgress = false;

            if (_currentRequest.result == UnityWebRequest.Result.Success)
            {
                string json = _currentRequest.downloadHandler.text;
                var jsonObject = JSON.Parse(json);

                if (jsonObject != null && jsonObject["data"] != null)
                {
                    var jsonArray = jsonObject["data"].AsArray;
                    (string, string)[] breedNames = new (string, string)[Math.Min(10, jsonArray.Count)];

                    for (int i = 0; i < breedNames.Length; i++)
                    {
                        var breedData = jsonArray[i];
                        string id = breedData["id"];
                        string name = breedData["attributes"]["name"];
                        breedNames[i] = (id, name);
                    }

                    OnBreedsLoaded?.Invoke(new BreedsArgs(breedNames));
                }
                else
                {
                    Debug.LogError("Данные о породах не найдены.");
                }
            }
            else
            {
                Debug.LogError($"Ошибка запроса: {_currentRequest.error}");
            }
        }
    }

    public void FetchBreedInfo(string breedId)
    {
        CancelRequest();
        _requestQueue.AddRequest(SendBreedInfoRequest(breedId));
    }

    IEnumerator SendBreedInfoRequest(string breedId)
    {
        _isRequestInProgress = true;
        using (_currentRequest = UnityWebRequest.Get($"https://dogapi.dog/api/v2/breeds/{breedId}"))
        {
            yield return _currentRequest.SendWebRequest();
            _isRequestInProgress = false;

            if (_currentRequest.result == UnityWebRequest.Result.Success)
            {
                string json = _currentRequest.downloadHandler.text;
                var jsonObject = JSON.Parse(json);

                if (jsonObject != null && jsonObject["data"] != null && jsonObject["data"]["attributes"] != null)
                {
                    var attributes = jsonObject["data"]["attributes"];

                    BreedInfoArgs args = new BreedInfoArgs(
                        attributes["name"],
                        attributes["description"]
                    );

                    OnDogInfoLoaded?.Invoke(args);
                }
                else
                {
                    Debug.LogError("Информация о породе не найдена.");
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
// тут я не писал классы json, так особо информации нет, но это можно расширить, если потребует задача
public class BreedsArgs:EventArgs
{
    public (string,string)[] breeds{get;}
    public BreedsArgs((string,string)[] _breeds)
    {
        breeds= _breeds;
    }
}
public class BreedInfoArgs:EventArgs
{
    public string name{get;}
    public string description{get;}
    public BreedInfoArgs(string _name, string _description)
    {
        name= _name;
        description= _description;
    }
}