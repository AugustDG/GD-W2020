using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public int direction;
    public Animator animator;

    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 150f;
    [SerializeField] private float maxScale = 5.15f;
    [SerializeField] private bool canJump = true;
    [SerializeField] private Transform spriteTransform;
    
    private Rigidbody _rigidbody;
    private AudioSource _audioSource;
    private bool _hasStarted;
    private Vector2[] _compass = new[] {Vector2.up, Vector2.right, Vector2.down, Vector2.left};

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (transform.position.z > 5f) SceneManager.LoadSceneAsync(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_hasStarted) return;
        
        if (canJump)
        {
            var axisValue = Input.GetAxis("Jump");
            _rigidbody.AddExplosionForce(jumpForce * axisValue, transform.position + Vector3.forward, 10f, 0f,
                ForceMode.Impulse); //adds an explosive force to make the player jump
            if (axisValue > 0.5f) canJump = false; //disables jumps
        }

        Vector2 velocity = _rigidbody.velocity.normalized;

        var maxDot = -Mathf.Infinity;
        var chosenVecIndex = -1;

        if (velocity.magnitude < 0.5f)
        {
            chosenVecIndex = -1;
        }
        else
        {
            for (var i = 0; i < 4; i++)
            {
                var temp = Vector2.Dot(velocity, _compass[i]);
                if (temp > maxDot)
                {
                    chosenVecIndex = i;
                    maxDot = temp;
                }
            }
        }

        if (chosenVecIndex != -1)
        {
            ResetAnimatorParameters();
            direction = chosenVecIndex;
        }

        switch (chosenVecIndex)
        {
            case -1:
                animator.SetBool("isIdle", true);
                break;
            case 0:
                animator.SetBool("isBack", true);
                break;
            case 1:
                animator.SetBool("isRight", true);
                break;
            case 2:
                animator.SetBool("isFront", true);
                break;
            case 3:
                animator.SetBool("isLeft", true);
                break;
        }

        _rigidbody.velocity = new Vector3(Input.GetAxis("Horizontal") * speed, Input.GetAxis("Vertical") * speed,
            _rigidbody.velocity.z); //moves the player with the supplied inputs

        var scale = Mathf.LerpUnclamped(5f, maxScale,
            Mathf.Abs(transform.position.z)); //changes size of sprite for "elevation effect"
        spriteTransform.localScale = new Vector3(scale, scale, 1f); //applies local scale to the sprite transform
    }

    private void OnCollisionEnter(Collision other)
    {
        //enables jump again
        if (other.transform.name == "Ground_Tilemap") canJump = true;
    }

    void ResetAnimatorParameters()
    {
        animator.SetBool("isIdle", false);
        animator.SetBool("isFront", false);
        animator.SetBool("isBack", false);
        animator.SetBool("isLeft", false);
        animator.SetBool("isRight", false);
    }

    public void PlaySound()
    {
        _audioSource.Play();
    }

    public void PlayerDied()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void GameStarted()
    {
        _hasStarted = true;
    }
}