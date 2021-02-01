using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Manager : MonoBehaviour
{
    public UnityEvent startGameEvent;
    public UnityEvent startMonsterEvent;

    [SerializeField] private GameObject[] gameUIToDisable;
    [SerializeField] private GameObject[] gameUIToEnable;

    public void PlayGame()
    {
        startGameEvent.Invoke();
                
        foreach (var t in gameUIToDisable)
        {
            t.SetActive(false);
        }
        
        foreach (var t in gameUIToEnable)
        {
            t.SetActive(true);
        }
    }

    public void QuitGame()
    {
        Application.Quit(69);
    }

    public IEnumerator DelayMonster()
    {
        yield return new WaitForSeconds(15f);
        startMonsterEvent.Invoke();
    }
}
