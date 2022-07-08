using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseStep : MonoBehaviour, IStep
{
    public StepStates StepState { get; protected set; } = StepStates.Incomplete;

    public void Progress()
    {
        DerivedProgress();
        OnProgress.Invoke();
    }

    public void Regress()
    {
        DerivedRegress();
        OnRegress.Invoke();
    }

    public virtual void Started()
    {

    }

    public virtual void Ended()
    {

    }

    protected virtual void DerivedProgress()
    {

    }

    protected virtual void DerivedRegress()
    {

    }

    public virtual void Triggered()
    {
        
    }

    public virtual void TriggeredTrue()
    {

    }

    [SerializeField] private UnityEvent onComplete = new UnityEvent();

    public UnityEvent OnComplete => onComplete;

    [SerializeField] private UnityEvent onProgress = new UnityEvent();

    public UnityEvent OnProgress => onProgress;

    [SerializeField] private UnityEvent onRegress = new UnityEvent();

    public UnityEvent OnRegress => onRegress;

    private UnityEvent onTriggered = new UnityEvent();

    public UnityEvent OnTriggered => onTriggered;
}
