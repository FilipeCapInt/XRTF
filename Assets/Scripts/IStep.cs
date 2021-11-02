using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IStep
{
    StepStates StepState { get; }

    void Progress();

    void Regress();

    void Started();

    void Ended();

    UnityEvent OnComplete { get; }

    UnityEvent OnProgress { get; }

    UnityEvent OnRegress { get; }
}
