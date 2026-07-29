using System;
using UnityEngine;

/// <summary>
/// Нужен что бы запустить проверку можно ли продолжить
/// </summary>
public abstract class AbsCheckContinue : MonoBehaviour
{
    public abstract event Action OnInit;
    public abstract bool IsInit { get; }
    
    /// <summary>
    /// Будут отражать закончила ли Task выполнение
    /// (Прикол в том, что если вызвать метод BreakCheck, то Task будет считаться завершенной, а вот Callback ещё нет)
    /// </summary>
    public abstract event Action OnCompletedLogic;
    public abstract bool IsCompletedLogic { get; }

    public abstract GetServerRequestData<DataIsContinue> LastCallback { get; }
    
    public abstract GetServerRequestData<DataIsContinue> CheckContinue(DKOKeyAndTargetAction dataDKO);

    /// <summary>
    /// Нужен на случай если дальше нету смысла продолжать проверки, а запущенные проверки надо остановить
    /// </summary>
    public abstract void BreakCheck();
}
