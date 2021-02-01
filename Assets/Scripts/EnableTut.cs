using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableTut : MonoBehaviour
{
    [SerializeField] private Manager manager;
    
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void StartGame()
    {
        _animator.enabled = true;
    }

    public void StartMonsterCount()
    {
        StartCoroutine(manager.DelayMonster());
    }
}
