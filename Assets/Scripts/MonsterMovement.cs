using System;
using System.Collections;
using System.Collections.Generic;
using SAP2D;
using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    [SerializeField] private Vector2 point;
    
    private Animator _animator;
    private SAP2DAgent _agent;
    private AudioSource _audioSource;
    private bool _hasStarted;
    private Vector2[] _compass = new[] {Vector2.up, Vector2.right, Vector2.down, Vector2.left};

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<SAP2DAgent>();
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_hasStarted) return;
        
        var chosenVecIndex = -1;

        if (_agent.path.Length > 0)
        {
            point = _agent.path[0] - (Vector2) transform.position;

            var maxDot = -Mathf.Infinity;

            if (point.magnitude < 0.05f)
            {
                chosenVecIndex = -1;
            }
            else
            {
                for (var i = 0; i < 4; i++)
                {
                    var temp = Vector2.Dot(point, _compass[i]);
                    if (temp > maxDot)
                    {
                        chosenVecIndex = i;
                        maxDot = temp;
                    }
                }
            }
        }

        ResetAnimatorParameters();

        switch (chosenVecIndex)
        {
            case -1:
                _animator.SetBool("isIdle", true);
                break;
            case 1:
            case 2:
                _animator.SetBool("isRightDown", true);
                break;
            case 0:
            case 3:
                _animator.SetBool("isLeftUp", true);
                break;
        }
    }

    void ResetAnimatorParameters()
    {
        _animator.SetBool("isIdle", false);
        _animator.SetBool("isRightDown", false);
        _animator.SetBool("isLeftUp", false);
    }

    public void StartMovement()
    {
        _hasStarted = true;
        _agent.CanMove = true;
    }

    public void PlayAudio()
    {
        print("run!");
        _audioSource.Play();
    }
}