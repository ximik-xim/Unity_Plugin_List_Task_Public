using System;
using UnityEngine;

public class TaskStartAndBreakBlockTimerTaskListTaskDKO : TL_AbsTaskLogicDKO
{
    [SerializeField]
    private bool _isStartBlockTimer;
    
    [SerializeField]
    private BlockTaskTimer _blockTaskTimer;

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
        
        if (_isStartBlockTimer == true) 
        {
            _blockTaskTimer.StratBlockTime();
        }
        else
        {
            _blockTaskTimer.BreakTimer();
        }
        
        _isCompletedLogic = true;
        OnCompletedLogic?.Invoke();
    }
}