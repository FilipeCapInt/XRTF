using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueStep : BaseStep
{
    [SerializeField]
    private Button Btn;
    [SerializeField]
    private Text nPCTextObject;
    [SerializeField]
    private Text playerTextObject;
    private bool hasPlayer = false;

    [SerializeField] int linesOfDialogue = 0;

    [SerializeField] private string[] NPCDialogue;

    [SerializeField] private string[] PlayerDialogue;

    public override void Started()
    {
        int dialogueCount = 0;
        nPCTextObject.text = NPCDialogue[dialogueCount];
        playerTextObject.text = PlayerDialogue[dialogueCount];

        Btn.onClick.RemoveListener(Dialogue);
        Btn.onClick.AddListener(Dialogue);
    }

    public override void Ended()
    {
        base.Ended();

        Btn.onClick.RemoveListener(DoneListener);
    }

    private int dialogueCount = 0;
    public void Dialogue()
    {
        dialogueCount++;
        Debug.Log(dialogueCount);
        if(dialogueCount <= linesOfDialogue-1)
        {
            nPCTextObject.text = NPCDialogue[dialogueCount];
            playerTextObject.text = PlayerDialogue[dialogueCount];
        }
        else
        {
            DoneListener();
        }  
        
    }

    private void DoneListener()
    {
        Debug.Log("Pressed button!");
        StepState = StepStates.Correct;

        OnComplete.Invoke();
    }
    public override void Triggered()
    {
        Btn.onClick.RemoveListener(TriggeredTrue);
        Btn.onClick.AddListener(TriggeredTrue);
    }
    public override void TriggeredTrue()
    {
        OnTriggered.Invoke();
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

