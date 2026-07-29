using System;
using UnityEngine;

public class PrintStatusCompletedTaskContinueDKO : MonoBehaviour
{
    [SerializeField]
    private LogicListTaskContinueDKO _listTask;

    private void Awake()
    {
        _listTask.OnCompleted += OnCompleted;
    }

    private void OnCompleted()
    {
        Debug.Log($"Статус условий {_listTask.LastCallback.GetData.IsContinue}, Ошиюки : {_listTask.LastCallback.GetData.TextError}" );
    }
}
