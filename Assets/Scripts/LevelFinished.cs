using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFinished : MonoBehaviour
{
    private IEnumerator OnTriggerEnter(Collider other)
    {
        yield return new WaitForSeconds(2.5f);
        
        SceneManager.LoadSceneAsync(0);
    }
}
