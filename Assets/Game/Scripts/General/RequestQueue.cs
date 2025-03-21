using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestQueue : MonoBehaviour
{
    Queue<IEnumerator> _queue = new Queue<IEnumerator>();
    bool _isProcessing = false;
    //все запросы реализованы в формате коротины чтобы выполнятьбез блокировки основного потока. 
    //Можно было использовать task или jobs, но опыта с ними у меня особо нет, и то только с процедурной генерацией
    //Запрсоы занимают время, и игра просто зависнет пока не получит ответ
    public void AddRequest(IEnumerator request)
    {
       
        _queue.Enqueue(request);
        if (!_isProcessing)
            StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        _isProcessing = true;
        while (_queue.Count > 0)
            yield return StartCoroutine(_queue.Dequeue());
        _isProcessing = false;
    }

    public void ClearQueue()
    {
        _queue.Clear();
    }
}
