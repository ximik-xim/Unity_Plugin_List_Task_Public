using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Нужен только для примера
/// для выбора какую из реализации проекций выполнения задач запустить
/// </summary>
public class TL_ExampleSelectUseReplicationTask : MonoBehaviour
{
    [SerializeField]
    private bool _startLogicTaskReadyExecutor;
    
    [SerializeField]
    private bool _startLogicTaskExecutorSequence;

    [SerializeField] 
    private TL_TaskReplicationWrapperMono _replicationTaskReadyExecutor;

    [SerializeField] 
    private TL_TaskReplicationWrapperMono _replicationTaskExecutorSequence;
    
    private void OnValidate()
    {
        if (_startLogicTaskReadyExecutor == true)
        {
            _replicationTaskReadyExecutor.StartAction();
        }
        
        if (_startLogicTaskExecutorSequence == true)
        {
            _replicationTaskExecutorSequence.StartAction();
        }
    }
}
