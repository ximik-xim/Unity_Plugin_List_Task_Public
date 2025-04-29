using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleStorageTaskExecutor : MonoBehaviour
{

    [SerializeField] 
    private bool _init;

    [SerializeField]
    private bool _startLogic;
    
    [SerializeField] 
    private TL_TaskExecutorSequence _task1;
    
    [SerializeField] 
    private TL_TaskReadyExecutorDontBuffer _task2;
    
    [SerializeField] 
    private TL_TaskReadyExecutorLocalBuffer _task3;
    
    [SerializeField] 
    private TL_TaskReadyExecutorBuffer _task4;


    private void Awake()
    {
        _task1.OnInit += OnInitTask;
        _task1.OnCompleted += OnCompletedTask;
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
            if (_task1.IsInit == false)
            {
                _task1.StartInit();
            }
        }
        
        if (_startLogic == true)
        {
            if (_task1.IsCompleted == true)
            {
                _task1.StartAction();
            }
        }
    }

    private void OnDestroy()
    {
        _task1.OnInit -= OnInitTask;
        _task1.OnCompleted -= OnCompletedTask;
    }
}
