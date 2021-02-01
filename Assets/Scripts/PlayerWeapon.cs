using System;
using System.Collections;
using System.Collections.Generic;
using SAP2D;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering.Universal;
using Random = UnityEngine.Random;

[Serializable]
public class WeaponInfo
{
    public int loadedAmmo;
    public int totalAmmo;
    public int clipSize;

    public float fireRate = 0.4f;
}

public class PlayerWeapon : MonoBehaviour
{
    public bool canShoot = true;
    public bool isHidden;

    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameObject shotPrefab;
    [SerializeField] private GameObject weapon;
    [SerializeField] private SAP2DAgent agent;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Light2D playerLight;
    [SerializeField] private AudioSource[] _gunSources = new AudioSource[2];
    [SerializeField] private WeaponInfo[] weapons;

    private Color _opaqueColor;
    private SpriteRenderer _pastBushRenderer;
    private SpriteRenderer _weaponRenderer;
    private int _currentWeapon;
    private bool _isWeaponSwitch;
    private Camera _main;
    private IEnumerator _curentHideRoutine;
    private IEnumerator _curentShootRoutine;
    private Animator _playerAnimator;
    private bool _hasStarted;

    private void Awake()
    {
        _weaponRenderer = GetComponentInChildren<SpriteRenderer>();
        _main = Camera.main;

        weapons = new[]
        {
            new WeaponInfo
            {
                clipSize = 6,
                totalAmmo = -1,
                loadedAmmo = 6
            },
            new WeaponInfo
            {
                clipSize = 5,
                totalAmmo = 2,
                loadedAmmo = 5
            },
            new WeaponInfo
            {
                clipSize = 3,
                totalAmmo = 3,
                loadedAmmo = 3
            }
        };
    }

    private void Start()
    {
        _playerAnimator = playerMovement.animator;

        if (_currentWeapon != 0)
            ammoText.text = $"{weapons[_currentWeapon].loadedAmmo}/{weapons[_currentWeapon].totalAmmo}";
        else
            ammoText.text = $"{weapons[_currentWeapon].loadedAmmo}/∞";
    }

    // Update is called once per frame
    void Update()
    {
        if (!_hasStarted) return;
        
        Vector3 lookAt = _main.ScreenToWorldPoint(Input.mousePosition);

        float angleRad = Mathf.Atan2(lookAt.y - playerMovement.transform.position.y,
            lookAt.x - playerMovement.transform.position.x);

        float angleDeg = CustomClamp(Mathf.Repeat((180 / Mathf.PI) * angleRad, 360f));

        transform.rotation = Quaternion.Euler(0, 0, angleDeg);

        if (Input.GetButtonDown("Fire1") && canShoot && weapons[_currentWeapon].loadedAmmo > 0)
        {
            shotPrefab.GetComponentInChildren<Shot>().shotType = _currentWeapon;

            var angleOffset = 0f;

            switch (playerMovement.direction)
            {
                case 0: //Back
                    if (!_isWeaponSwitch)
                        angleOffset = -45f;
                    else
                        angleOffset = -135f;
                    break;
                case 3: //Left
                    angleOffset = -45f;
                    break;
                case 2: //Front
                case 1: //Right
                    if (!_isWeaponSwitch)
                        angleOffset = 225f;
                    else
                        angleOffset = 315f;
                    break;
            }

            if (_currentWeapon == 2)
            {
                Instantiate(shotPrefab, weapon.transform.position,
                    Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y,
                        transform.rotation.eulerAngles.z + angleOffset - 10));
                Instantiate(shotPrefab, weapon.transform.position,
                    Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y,
                        transform.rotation.eulerAngles.z + angleOffset));
                Instantiate(shotPrefab, weapon.transform.position,
                    Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y,
                        transform.rotation.eulerAngles.z + angleOffset + 10));
            }
            else
            {
                Instantiate(shotPrefab, weapon.transform.position,
                    Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y,
                        transform.rotation.eulerAngles.z + angleOffset));   
            }

            _gunSources[0].Play();
            if (isHidden) ShowPlayer(true);

            weapons[_currentWeapon].loadedAmmo--;

            if (_currentWeapon != 0)
                ammoText.text = $"{weapons[_currentWeapon].loadedAmmo}/{weapons[_currentWeapon].totalAmmo}";
            else
                ammoText.text = $"{weapons[_currentWeapon].loadedAmmo}/∞";

            _curentShootRoutine = ShotRate();

            StartCoroutine(_curentShootRoutine);
        }

        if (Input.GetKeyDown(KeyCode.R) &&
            (weapons[_currentWeapon].totalAmmo > 0 || weapons[_currentWeapon].totalAmmo == -1))
        {
            StartCoroutine(Reload());
        }
    }

    private float CustomClamp(float value)
    {
        switch (playerMovement.direction)
        {
            case 0: //Back
                if (value >= 0f && value < 90f)
                {
                    _isWeaponSwitch = true;
                    _playerAnimator.SetBool("WeaponSwitch", _isWeaponSwitch);
                    return value - 45f;
                }
                else if (value >= 90f && value <= 180f)
                {
                    _isWeaponSwitch = false;
                    _playerAnimator.SetBool("WeaponSwitch", _isWeaponSwitch);
                    return value - 135f;
                }
                else if (value > 270f && value <= 360f) return 315f;
                else return 45f;
            case 1: //Right
                if (value >= 0f && value < 90f)
                {
                    return value - 45f;
                }
                else if (value >= 270f && value <= 360f)
                {
                    _isWeaponSwitch = false;
                    _playerAnimator.SetBool("WeaponSwitch", _isWeaponSwitch);
                    return value - 45f;
                }
                else if (value > 180f && value <= 270f) return 225f;
                else return 45f;
            case 2: //Front
                if (value >= 180f && value < 270f)
                {
                    _isWeaponSwitch = true;
                    _playerAnimator.SetBool("WeaponSwitch", _isWeaponSwitch);
                    return value - 135f;
                }
                else if (value >= 270f && value <= 360f)
                {
                    _isWeaponSwitch = false;
                    _playerAnimator.SetBool("WeaponSwitch", _isWeaponSwitch);

                    return value - 45f;
                }
                else if (value > 90f && value <= 180f) return 45f;
                else return 315f;
            case 3: //Left
                if (value >= 90f && value < 270f)
                {
                    return value - 135f;
                }
                else if (value > 270f && value <= 360f) return 135f;
                else return -45f;
            default:
                return value;
        }
    }

    public void GameStarted()
    {
        _hasStarted = true;
    }
    
    public void ChangeWeapon(int currWeapon, Sprite currSprite)
    {
        _currentWeapon = currWeapon;
        _weaponRenderer.sprite = currSprite;

        if (_currentWeapon != 0)
            ammoText.text = $"{weapons[_currentWeapon].loadedAmmo}/{weapons[_currentWeapon].totalAmmo}";
        else
            ammoText.text = $"{weapons[_currentWeapon].loadedAmmo}/∞";
    }

    public void HidePlayer(SpriteRenderer bush)
    {
        _pastBushRenderer = bush;

        isHidden = true;

        _opaqueColor = _pastBushRenderer.color;
        var color = new Color(_opaqueColor.r, _opaqueColor.g, _opaqueColor.b, 0.7f);
        _pastBushRenderer.color = color;
        playerLight.intensity = 0.22f;

        _curentHideRoutine = RandomizeTargetPos();

        StartCoroutine(_curentHideRoutine);

        agent.Target = targetTransform;
    }

    public void ShowPlayer(bool hasShot)
    {
        isHidden = false;

        if (!hasShot)
        {
            var color = new Color(_opaqueColor.r, _opaqueColor.g, _opaqueColor.b, 1f);
            _pastBushRenderer.color = color;
        }
        else
        {
            _pastBushRenderer.gameObject.SetActive(false);

            StartCoroutine(PlayerExposed());
        }

        playerLight.intensity = 1f;

        StopCoroutine(_curentHideRoutine);

        agent.Target = playerMovement.transform;
    }

    private IEnumerator PlayerExposed()
    {
        yield return new WaitForSeconds(2.5f);

        _pastBushRenderer.gameObject.SetActive(true);
    }

    private IEnumerator RandomizeTargetPos()
    {
        var player = playerMovement.transform.position;
        Vector3 target;
        target = player + new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
        targetTransform.position = target;

        yield return new WaitForSeconds(2f);

        _curentHideRoutine = RandomizeTargetPos();

        StartCoroutine(_curentHideRoutine);
    }

    private IEnumerator ShotRate()
    {
        canShoot = false;
        yield return new WaitForSeconds(weapons[_currentWeapon].fireRate);
        canShoot = true;
    }

    private IEnumerator Reload()
    {
        StopCoroutine(_curentShootRoutine);

        canShoot = false;

        var ammoBefore = weapons[_currentWeapon].loadedAmmo;

        for (int i = 1; i <= weapons[_currentWeapon].clipSize - ammoBefore; i++)
        {
            if (weapons[_currentWeapon].totalAmmo == -1)
            {
                weapons[_currentWeapon].loadedAmmo++;

                ammoText.text = $"{weapons[_currentWeapon].loadedAmmo}/∞";
            }
            else if (weapons[_currentWeapon].totalAmmo > 0)
            {
                weapons[_currentWeapon].totalAmmo--;
                weapons[_currentWeapon].loadedAmmo++;

                ammoText.text = $"{weapons[_currentWeapon].loadedAmmo}/{weapons[_currentWeapon].totalAmmo}";
            }
            else
            {
                break;
            }

            _gunSources[1].Stop();
            _gunSources[1].Play();

            yield return new WaitForSeconds(0.5f);
        }

        canShoot = true;
    }
}