using System;
using System.Collections;
using System.Collections.Generic;
using SAP2D;
using UnityEngine;
using UnityEngine.Events;

public class TriggerManager : MonoBehaviour
{
    public UnityEvent damaged;
    public float slowRate = .75f;
    
    private SAP2DAgent _agent;
    private Collider _collider;
    private bool _canAttack = true;
    
    private void Start()
    {
        _agent = GetComponent<SAP2DAgent>();
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Shot"))
        {
            StartCoroutine(GotShot());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name.Equals("Player") && _canAttack)
        {
            if (!other.GetComponentInChildren<PlayerWeapon>().isHidden)
            {
                damaged.Invoke();
                StartCoroutine(DelayAttack());
            }
        }
    }

    IEnumerator GotShot()
    {
        _agent.MovementSpeed -= slowRate;
        
        yield return new WaitForSeconds(10f);
        
        _agent.MovementSpeed += slowRate;
    }

    IEnumerator DelayAttack()
    {
        _canAttack = false;
        
        yield return new WaitForSeconds(0.5f);

        _canAttack = true;
    }
}
