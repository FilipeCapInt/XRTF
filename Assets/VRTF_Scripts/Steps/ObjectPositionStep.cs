using System.Collections;
using UnityEngine;

public class ObjectPositionStep : BaseStep
{
    [SerializeField] private GameObject positionObject = null;
    [SerializeField] private Vector3 endPosition = new Vector3(0, 0, 0);
    [SerializeField] private Material instantiatedMaterial;
    private GameObject instantiatedObject;
    private bool trigger = false;
    public override void Started()
    {
        base.Started(); 
        StartCoroutine(CheckPosition());
        instantiatedObject = Instantiate(positionObject);
        instantiatedObject.transform.position = endPosition;
        instantiatedObject.GetComponentInChildren<MeshRenderer>().material = instantiatedMaterial;
        instantiatedObject.GetComponentInChildren<Collider>().enabled= false;
        instantiatedObject.GetComponent<Collider>().enabled = false;
    }
    public IEnumerator CheckPosition()
    {
        
        yield return new WaitUntil(() => positionObject.transform.position == endPosition);
        if (positionObject.transform.position == endPosition)
        {
            DoneListener();
            TriggeredTrue();
        }
        
    }

    public override void Ended()
    {
        base.Ended();
    }

    private void DoneListener()
    {
        Debug.Log("Position Correct", this);

        Destroy(instantiatedObject);

        StepState = StepStates.Correct;
        
        OnComplete.Invoke();
    }
    public override void Triggered()
    {
        trigger = true;
    }

    public override void TriggeredTrue()
    {
        if(trigger == true)
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

