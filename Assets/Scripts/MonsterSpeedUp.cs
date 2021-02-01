using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpeedUp : MonoBehaviour
{
    [SerializeField] private MonsterMovement movement;
    
    private void OnTriggerEnter(Collider other)
    {
        movement.StartMovement();
    }
}
