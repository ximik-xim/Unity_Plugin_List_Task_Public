
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ExampleStorageTaskExecutorWrapper : MonoBehaviour
{
    [SerializeField] 
    private bool _init;

    [SerializeField]
    private bool _startLogic;
    
    [SerializeField] 
    private TL_TaskWrapperMono _wrapperTaskExecutor;
    
    private void Awake()
    {
        _wrapperTaskExecutor.OnInit += OnInitTask;
        _wrapperTaskExecutor.OnCompleted += OnCompletedTask;
    }

    private void OnCompletedTask()
    {
        Debug.Log("Выполнение тасок прошло успешно");
    }

    private void OnInitTask()
    {
        Debug.Log("Инициализация тасок прошла успешно");
    }

    private void OnValidate()
    {
        if (_init == true)
        {
            if (_wrapperTaskExecutor.IsInit == false)
            {
                _wrapperTaskExecutor.StartInit();
            }
        }
        
        if (_startLogic == true)
        {
            if (_wrapperTaskExecutor.IsCompleted == true)
            {
                _wrapperTaskExecutor.StartAction();
            }
        }
    }

    private void OnDestroy()
    {
        _wrapperTaskExecutor.OnInit -= OnInitTask;
        _wrapperTaskExecutor.OnCompleted -= OnCompletedTask;
    }
}
