using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueStep : MonoBehaviour
{
    [SerializeField]
    private GameObject _UI;
    [SerializeField]
    private GameObject _ResponseButton;
    [SerializeField]
    private Text nPCTextObject;
    [SerializeField]
    private Text playerTextObject;
    private bool hasPlayer = false;

    [SerializeField] int linesOfDialogue = 0;

    [SerializeField] private string[] NPCDialogue;

    [SerializeField] private string[] PlayerDialogue;

    private void Start()
    {
        NPCDialogue = new string[linesOfDialogue];
        PlayerDialogue = new string[linesOfDialogue];

    }
    private int dialogueCount = -1;
    public void ChangeText()
    {
        dialogueCount++;
        Debug.Log(dialogueCount);
        switch (dialogueCount)
        {
           
        }
    }

    public void StepComplete(GameObject gameObject)
    {
        gameObject.GetComponent<BaseStep>();
    }
}
