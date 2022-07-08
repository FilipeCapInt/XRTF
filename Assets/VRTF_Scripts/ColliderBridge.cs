using BNG;
using System.Collections;
using UnityEngine;

public class ColliderBridge : MonoBehaviour
{
    [SerializeField] public ObjectInteractStep _listener;
    private GameObject player;
    private Grabber _grabber;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        _grabber = player.GetComponentInChildren<Grabber>();
    }


    private void OnCollisionEnter(Collision collision)
    {
       // _listener.CheckCollision();
    }
}

