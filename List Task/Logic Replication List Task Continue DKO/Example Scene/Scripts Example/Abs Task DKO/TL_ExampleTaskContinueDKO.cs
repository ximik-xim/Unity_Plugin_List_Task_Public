using System;
using System.Collections;
using UnityEngine;

public class TL_ExampleTaskContinueDKO : AbsCheckContinue
{
    public override event Action OnInit;
    public override bool IsInit => true;
    
    public override bool IsCompletedLogic => _isCompletedLogic;
    private bool _isCompletedLogic = true;
    public override event Action OnCompletedLogic;

    public override GetServerRequestData<DataIsContinue> LastCallback => _lastCallback.DataGet;
    private CallbackRequestDataWrapperT<DataIsContinue> _lastCallback;
    
    [SerializeField] 
    private float _secondComleted = 1f;
    private IEnumerator _enumeratorCompleted;

    [SerializeField]
    private bool _isContinue;
    
    [SerializeField]
    private string _textError;
    
    public override GetServerRequestData<DataIsContinue> CheckContinue(DKOKeyAndTargetAction dataDKO)
    {
        if (_isCompletedLogic == true)
        {
            _isCompletedLogic = false;

            CallbackRequestDataWrapperT<DataIsContinue> _wrapperCallbackData = new CallbackRequestDataWrapperT<DataIsContinue>(0);

            _lastCallback = _wrapperCallbackData;
            //Нужно, что бы при вызове Callbak задача закончилась(если BreakCheck не прелитит раньше)
            _wrapperCallbackData.DataGet.OnGetDataCompleted += OnCompleted;

            Debug.Log("Начало выполнение задачи. Ждите");
        
            _enumeratorCompleted = StartCompletedTask();
            StartCoroutine(_enumeratorCompleted);
            
            return _wrapperCallbackData.DataGet;
        }

        return null;
    }
    

    private void OnCompleted()
    {
        Completed();
    }
   
    private void Completed()
    {
        _isCompletedLogic = true;
        OnCompletedLogic?.Invoke();
    }
   
    /// <summary>
    /// Прерываем выполнение
    /// </summary>
    public override void BreakCheck()
    {
        _lastCallback.DataGet.OnGetDataCompleted -= OnCompleted;
        StopCoroutine(_enumeratorCompleted);
        
        Completed();
    }
    
    private IEnumerator StartCompletedTask()
    {
        yield return new WaitForSeconds(_secondComleted);

        DataIsContinue data;
        
        if (_isContinue == true)
        {
            data = new DataIsContinue(true);
        }
        else
        {
            data = new DataIsContinue(false, _textError);   
        }
        
        
        _lastCallback.Data.StatusServer = StatusCallBackServer.Ok;
        _lastCallback.Data.GetData = data;

        _lastCallback.Data.IsGetDataCompleted = true;
        _lastCallback.Data.Invoke();
        
        Debug.Log("Выполнение задачи закончено");
    }
    
    private void OnDestroy()
    {
        if (_enumeratorCompleted != null) 
        {
            StopCoroutine(_enumeratorCompleted);
        }
    }
}
