using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteractStep : BaseStep
{
    [SerializeField] private GameObject interactObject = null;
    [SerializeField] private Material interactMaterial;
    private List<Material> meshSave = new List<Material>();
    private bool trigger = false;
    private bool started = false;
    private GameObject player;
    private Grabber _grabber;
    private Grabbable interactGrab;
    public override void Started()
    {
        base.Started();

        started = true;

        interactGrab = interactObject.GetComponent<Grabbable>();

        player = GameObject.FindWithTag("Player");
        
        _grabber = player.GetComponentInChildren<Grabber>();
        
        MeshRenderer[] mesh = interactObject.GetComponentsInChildren<MeshRenderer>();
       
        int i = 0;
        
        foreach (MeshRenderer render in mesh)
        {
            meshSave.Add(render.material);
            render.material = interactMaterial;
            i++;
        }

        StartCoroutine(GrabbableActive());
    }

    private IEnumerator GrabbableActive()
    {
        if(interactGrab.BeingHeld == true)
        {
            CheckCollision();
        }

        if (interactGrab.RecentlyCollided == true)
        {
            //CheckCollision();
        }
        
        yield return new WaitUntil(() => _grabber.HoldingItem);
    }

    public void CheckCollision()
    {
        TriggeredTrue();
        if (started == true)
        {
            DoneListener();
        }
    }

    public override void Ended()
    {
        base.Ended();
    }

    private void DoneListener()
    {
        started = false;

        MeshRenderer[] mesh = interactObject.GetComponentsInChildren<MeshRenderer>();
        int i = 0;
        foreach (MeshRenderer render in mesh)
        {
            render.material = meshSave[i];
            i++;
        }

        Debug.Log("Triggered Interaction", this);

        StepState = StepStates.Correct;

        OnComplete.Invoke();
    }
    public override void Triggered()
    {
        trigger = true;
    }

    public override void TriggeredTrue()
    {
        if (trigger == true)
        {
            OnTriggered.Invoke();
            trigger = false;
        }
    }

    protected override void DerivedProgress()
    {
        base.DerivedProgress();
    }

    protected override void DerivedRegress()
    {
        base.DerivedRegress();
    }
}