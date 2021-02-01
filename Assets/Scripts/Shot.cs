using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{
    public int shotType;

    [SerializeField] private int timer;
    
    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        
        StartCoroutine(DestroyCountdown());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (shotType != 1)
            transform.Translate(Vector2.left);
        else if (shotType == 1)
            transform.Translate(Vector2.left*2f);
    }

    private IEnumerator DestroyCountdown()
    {
        for (int i = 0; i < timer; i++)
        {
            yield return new WaitForSeconds(1f);
        }
        
        Destroy(gameObject);
    }
}
