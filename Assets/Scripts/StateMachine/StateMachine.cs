using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class StateMachine
{
#if UNITY_EDITOR
    //cai nay de biet duoc no dang o state nao
    public string name;
#endif

    public delegate void StateAction(ref Action onEnter, ref Action onExecute, ref Action onExit);
    private Action onEnter, onExecute, onExit;
    private StateAction c_StateMachine;
    private StateAction p_StateMachine;

    public void Execute()
    {
        onExecute?.Invoke();
    }

    public void ChangeState(StateAction stateAction)
    {
        onExit?.Invoke();
        stateAction.Invoke(ref onEnter, ref onExecute, ref onExit);
        onEnter?.Invoke();
        p_StateMachine = c_StateMachine;
        c_StateMachine = stateAction;

#if UNITY_EDITOR
        //cai nay de biet duoc no dang o state nao
        name = stateAction.Method.Name;
#endif
    }

    public bool IsState(StateAction stateAction) => c_StateMachine == stateAction;
    public bool IsP_State(StateAction stateAction) => p_StateMachine == stateAction;

}


//private void State(ref Action onEnter, ref Action onExecute, ref Action onExit)
//{
//    onEnter = () =>
//    {

//    };

//    onExecute = () =>
//    {

//    };

//    onExit = () =>
//    {

//    };
//}