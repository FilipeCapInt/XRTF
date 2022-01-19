using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traits : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 100;

    [SerializeField]
    [Range(1, 10)]
    private int damagePerSecond = 3;

    [SerializeField]
    private string planetName;

    [SerializeField]
    [TextArea]
    //[Multiline]
    private string planetDescription;
}
