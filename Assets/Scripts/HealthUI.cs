using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public int playerHealth = 2;
    public UnityEvent died;

    [SerializeField] private RawImage[] heartImgs = new RawImage[3];
    [SerializeField] private Texture[] heartTexs = new Texture[2];

    public void PlayerDamaged()
    {
        if (playerHealth > 1)
        {
            playerHealth--;

            heartImgs[playerHealth].texture = heartTexs[0];
        }
        else
        {
            died.Invoke();
        }
    }
}
