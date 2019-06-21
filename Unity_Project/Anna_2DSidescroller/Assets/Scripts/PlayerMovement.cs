using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController2D controller;

    public float runSpeed = 40f;

    float horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;

    Animator animator;

    //bool _isPlaying_Walking = false;
    bool _isPlaying_Crouching = false;
    bool _isPlaying_Jumping = false;

    const int STATE_IDLE = 0;
    const int STATE_WALKING = 1;
    const int STATE_CROUCHING = 2;
    const int STATE_JUMPING = 3;

    int currentAnimationState = STATE_IDLE;


    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }

        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;

        }

        if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
        }

        if (jump && !_isPlaying_Crouching && !controller.keepCrouch)
        {
            changeState(STATE_JUMPING);
        }
        else if (crouch && controller.m_Grounded || controller.keepCrouch)
        {
            changeState(STATE_CROUCHING);
        }
        else if (Input.GetAxisRaw("Horizontal") != 0 && !_isPlaying_Jumping && controller.m_Grounded && !controller.keepCrouch)
        {
            changeState(STATE_WALKING);

        }
        else
        {
            if (controller.m_Grounded)
            {
                changeState(STATE_IDLE);
            }
        }
        //Debug.Log(currentAnimationState);
        //if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walking"))
        //{
        //    _isPlaying_Walking = true;
        //}
        //else _isPlaying_Walking = false;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jumping"))
        {
            _isPlaying_Jumping = true;
        }
        else _isPlaying_Jumping = false;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Crouching"))
        {
            _isPlaying_Crouching = true;
        }
        else _isPlaying_Crouching = false;

    }

    void FixedUpdate()
    {
        // Move our character
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;
    }
    private void changeState(int state)
    {
        if (currentAnimationState == state)
        {
            return;
        }
        switch (state)
        {
            case STATE_IDLE:
                animator.SetInteger("state", STATE_IDLE);
                break;
            case STATE_WALKING:
                animator.SetInteger("state", STATE_WALKING);
                break;
            case STATE_CROUCHING:
                animator.SetInteger("state", STATE_CROUCHING);
                break;
            case STATE_JUMPING:
                animator.SetInteger("state", STATE_JUMPING);
                break;
        }
        currentAnimationState = state;
    }
}