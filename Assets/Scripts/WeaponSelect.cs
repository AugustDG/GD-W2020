using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class IntSpriteEvent : UnityEvent<int, Sprite> { }

public class WeaponSelect : MonoBehaviour
{
    public int selectedWeapon;
    public IntSpriteEvent weaponChangedEvent = new IntSpriteEvent();
    
    [SerializeField] private Image[] weaponImages = new Image[3];
    [SerializeField] private Sprite[] weaponSprites = new Sprite[3];

    private int _previousWeapon;
    private bool _hasStarted;

    // Update is called once per frame
    void Update()
    {
        if (!_hasStarted) return;
        
        if (Input.mouseScrollDelta.y > 0)
        {
            if (selectedWeapon < 2)
                selectedWeapon++;
            else
                selectedWeapon = 0;
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            if (selectedWeapon > 0)
                selectedWeapon--;
            else
                selectedWeapon = 2;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
            selectedWeapon = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            selectedWeapon = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            selectedWeapon = 2;

        switch (selectedWeapon)
        {
            case 0:
                weaponImages[0].sprite = weaponSprites[0];
                weaponImages[1].sprite = weaponSprites[1];
                weaponImages[2].sprite = weaponSprites[2];
                break;
            case 1:
                weaponImages[0].sprite = weaponSprites[1];
                weaponImages[1].sprite = weaponSprites[2];
                weaponImages[2].sprite = weaponSprites[0];
                break;
            case 2:
                weaponImages[0].sprite = weaponSprites[2];
                weaponImages[1].sprite = weaponSprites[0];
                weaponImages[2].sprite = weaponSprites[1];
                break;
        }

        if (_previousWeapon != selectedWeapon)
        {
            weaponChangedEvent.Invoke(selectedWeapon, weaponSprites[selectedWeapon]);
        }

        _previousWeapon = selectedWeapon;
    }

    public void StartGame()
    {
        _hasStarted = true;
    }
}