using System;
using UnityEngine;

public class TaskSetAndRemoveBlockTaskListTaskDKO : TL_AbsTaskLogicDKO
{
    [SerializeField]
    private bool _isSetBlock;
    
    [SerializeField]
    private SBI_SetAndRemoveTask _setAndRemoveTask;

    public override event Action OnInit;
    public override bool IsInit => true;

    public override bool IsCompletedLogic => _isCompletedLogic;
    private bool _isCompletedLogic = false;
    public override event Action OnCompletedLogic;

    private void Awake()
    {
        OnInit?.Invoke();
    }

    public override void StartLogic(DKOKeyAndTargetAction dataDKO)
    {
        _isCompletedLogic = false;
        
        if (_isSetBlock == true) 
        {
            _setAndRemoveTask.SetTask();
        }
        else
        {
            _setAndRemoveTask.RemoveTask();
        }
        
        _isCompletedLogic = true;
        OnCompletedLogic?.Invoke();
    }
}
