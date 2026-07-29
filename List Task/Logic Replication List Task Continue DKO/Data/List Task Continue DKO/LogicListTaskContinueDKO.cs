using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Список задач с логикой на проверку выполнения условий, нужно что бы легко состовлять логические цепочки с проверкой данных
///
/// Если включен breakIsContinueFalse, то при первом же несоответствии,
/// выполнение задач будет прекращено и верну в Continue = false
///
/// Если выключен breakIsContinueFalse, то будет просто собирать ошибки, 
/// но в конце выполн. задач, все равно вернет Continue = false (если встретилась хотя бы 1 невыполн. задачи)
/// </summary>
public class LogicListTaskContinueDKO : MonoBehaviour
{
    public bool IsInit => _isInit;
    private bool _isInit = false;
    
    public event Action OnInit;

    
    [SerializeField]
    private List<AbsCheckContinue> _listTask;

    /// <summary>
    /// Сообщает об успешном выполнении всех указанных задач
    /// </summary>
    public event Action OnCompleted;
    public bool IsCompleted => _isCompleted;
    private bool _isCompleted = true;

    
    /// <summary>
    /// Тут определяю проекцию поведения
    /// (по очереди будут выполняться задачи или как получиться)
    /// </summary>
    [SerializeField]
    private TL_AbsTaskReplicationWrapperMono _replicationTask;
    
    
     /// <summary>
    /// Ссылка на DKO таила
    /// (сохраняеться до конца действий)
    /// </summary>
    private DKOKeyAndTargetAction _bufferDKO;


    /// <summary>
    /// Буду ли прекращать выполнение остальных задач, если хотя бы 1 задача пришла с данными Continue = false
    /// </summary>
    [SerializeField]
    private bool _breakIsContinueFalse;

    public GetServerRequestData<DataIsContinue> LastCallback => _lastCallback.DataGet;
    private CallbackRequestDataWrapperT<DataIsContinue> _lastCallback;
    
    private void Awake()
    {
        _replicationTask.OnCheckCompleted += OnCheckCompleted;
        _replicationTask.OnStartAction += OnStartAction;
        _replicationTask.OnCompleted += OnCompletedLogic;
        _replicationTask.OnCompletedElement += OnElementCompleted;

        StartInit();
    }
    
    /// <summary>
    /// Логика проверки готовности задач при включении скрипта
    /// </summary>
    private void StartInit()
    {
        List<AbsCheckContinue> _buffer = new List<AbsCheckContinue>();
        bool _isStart = false;
        
        StartLogic();

        void StartLogic()
        {
            if (_isInit == false)
            {
                _isStart = true;

                foreach (var VARIABLE in _listTask)
                {
                    //нужно именно 2 проверки
                    //1 - что бы не вызвать Init просто так(обьект может сам инициализироваться отдельно)
                    //2 - что бы проверить инициализировался ли обьект или его нужно ждать
                    if (VARIABLE.IsInit == false)
                    {
                        if (VARIABLE.IsInit == false)
                        {
                            _buffer.Add(VARIABLE);
                            VARIABLE.OnInit += CheckInitCompleted;
                        }
                    }
                }

                _isStart = false;

                CheckInitCompleted();
            }
        }

        void CheckInitCompleted()
        {
            if (_isStart == false) 
            {
                int targetCount = _buffer.Count;
                for (int i = 0; i < targetCount; i++)
                {
                    if (_buffer[i].IsInit == true)
                    {
                        _buffer[i].OnInit -= CheckInitCompleted;
                        _buffer.RemoveAt(i);
                        i--;
                        targetCount--;
                    }
                }

                if (_buffer.Count == 0)
                {
                    InitCompleted();
                }
            }
        }
    }

    private void InitCompleted()
    {
        _isInit = true;
        OnInit?.Invoke();
    }
    
    public GetServerRequestData<DataIsContinue> StartAction(DKOKeyAndTargetAction tileDKO)
    {
        //Только если все операции были выполнены, тогда разрешаю запуск
        if (IsCompleted == true)
        {
            CallbackRequestDataWrapperT<DataIsContinue> _wrapperCallbackData = new CallbackRequestDataWrapperT<DataIsContinue>(0);
            _lastCallback = _wrapperCallbackData;
            
            _bufferDKO = tileDKO;
            
            //тут ищу задачи которые еще не закончили выполнение, и сохраняю их Id из списка 
            List<int> id = new List<int>();
 
            for (int i = 0; i < _listTask.Count; i++)
            {
                id.Add(i);
               
                _listTask[i].OnCompletedLogic += OnCompletedElement;
            }
        
        
            //Запускаю выполнение задачи
            _replicationTask.StartAction(id);

            return _lastCallback.DataGet;
        }

        return null;
    }

        
    private void OnElementCompleted(int id)
    {
        _listTask[id].OnCompletedLogic -= OnCompletedElement;
    }
   
    /// <summary>
    ///Когда задача выполниться, она сообщит об этом в логику по выполнению задач
    /// </summary>
    private void OnCompletedElement()
    {
        _replicationTask.ActionCompleted();
    }
    
    /// <summary>
    /// Тут, логика выполнение задач определяет какой будет вызван следующий элемент
    /// </summary>
    private void OnStartAction(int id)
    {
        _listTask[id].CheckContinue(_bufferDKO);
    }

    /// <summary>
    /// Тут логика задач проверяет какая из задач уже выполнена, а какие еще нет
    /// </summary>
    private bool OnCheckCompleted(int id)
    {
        if (_listTask[id].IsCompletedLogic == true)
        {
            //Если в callback вернули, что условие не было выполнено(нельзя продолжать действия)
            if (_listTask[id].LastCallback.GetData.IsContinue == false)
            {
                //Если включено прекращение выполнение операции, при первом же невыполнении условия, то заканчиваем выполнение
                if (_breakIsContinueFalse == true) 
                {
                    
                    foreach (var VARIABLE in  _listTask)
                    {
                        VARIABLE.OnCompletedLogic -= OnCompletedElement;
                    }
                    
                    _replicationTask.Break(false);
                    
                    _isCompleted = true;
                    
                    _lastCallback.Data.StatusServer = StatusCallBackServer.Ok;
                    _lastCallback.Data.GetData = _listTask[id].LastCallback.GetData;

                    _lastCallback.Data.IsGetDataCompleted = true;
                    _lastCallback.Data.Invoke();

                    OnCompleted?.Invoke();
                    

                    return true;
                }
                else
                {
                    //Иначе, просто собираю ошибки
                    if (_lastCallback.Data.GetData == null)
                    {
                        _lastCallback.Data.GetData = new DataIsContinue(false, _listTask[id].LastCallback.GetData.TextError);
                    }
                    else
                    {
                        _lastCallback.Data.GetData = new DataIsContinue(false, _lastCallback.Data.GetData.TextError + "\n" + _listTask[id].LastCallback.GetData.TextError);    
                    }
                                        
                }
            }
        }
        
        return _listTask[id].IsCompletedLogic;
    }
    
    /// <summary>
    /// сработает когда все задачи будут выполнены в логике задач
    /// </summary>
    private void OnCompletedLogic()
    {
        //Если выключено прекращение выполнение операции, при первом же невыполнении условия
        if (_breakIsContinueFalse == false)
        {
            // если данные пустые, то значит не заносили ошибки, и все прошло штатно
            if (_lastCallback.Data.GetData == null) 
            {
                _isCompleted = true;

                _lastCallback.Data.StatusServer = StatusCallBackServer.Ok;
                _lastCallback.Data.GetData = new DataIsContinue(true);

                _lastCallback.Data.IsGetDataCompleted = true;
                _lastCallback.Data.Invoke();
                
                OnCompleted?.Invoke();
            }
            else
            {
                //иначе, просто все занесенные ошибки отправляю
                _isCompleted = true;
                
                _lastCallback.Data.StatusServer = StatusCallBackServer.Ok;

                _lastCallback.Data.IsGetDataCompleted = true;
                _lastCallback.Data.Invoke();
                
                OnCompleted?.Invoke();
            }
        }
        else
        {
            //Если включено прекращение выполнение операции, и попали сюда, то значит все опер. завершились без проблем
            _isCompleted = true;
            
            _lastCallback.Data.StatusServer = StatusCallBackServer.Ok;
            _lastCallback.Data.GetData = new DataIsContinue(true);

            _lastCallback.Data.IsGetDataCompleted = true;
            _lastCallback.Data.Invoke();
                
            OnCompleted?.Invoke();
        }
    }

    private void OnDestroy()
    {
        _replicationTask.OnCheckCompleted -= OnCheckCompleted;
        _replicationTask.OnStartAction -= OnStartAction;
        _replicationTask.OnCompleted -= OnCompletedLogic;
        _replicationTask.OnCompletedElement -= OnElementCompleted;

    }
}
