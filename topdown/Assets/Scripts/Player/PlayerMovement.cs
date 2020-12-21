﻿using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool canMove = true;
    public float baseSpeed = 3f;
    public float dashSpeed = 100f;
    public float dashTime = 0.1f;
    public float speed;
    public Rigidbody2D rb;
    public Animator animator;

    private Vector2 direction;
    private PlayerController playerController;
    private float dashTimeLeft;
    private void Awake()
    {
        speed = baseSpeed;
        playerController = GetComponent<PlayerController>();
        playerController.resetStats.AddListener(ResetStats);
        FloorGlobal.Instance.levelChanged.AddListener(SaveSpeed);
        playerController.loadPlayerData.AddListener(LoadSpeed);
    }

    private void ResetStats()
    {
        speed = baseSpeed;
    }

    private void SaveSpeed()
    {
        ES3.Save("playerSpeed", speed);
    }

    private void LoadSpeed()
    {
        speed = ES3.Load("playerSpeed", baseSpeed);
    }

    public void Dash()
    {
        if (!playerController.isDashing)
        {
            playerController.isDashing = true;
            playerController.isInvincible = true;
            dashTimeLeft = dashTime;
        }
    }

    private void CheckDash()
    {
        if (playerController.isDashing)
        {
            if(dashTimeLeft > 0)
            {
                canMove = false;
                rb.velocity = dashSpeed * direction;
                dashTimeLeft -= Time.deltaTime;
            }
            if(dashTimeLeft <= 0)
            {
                playerController.isDashing = false;
                playerController.isInvincible = false;
                canMove = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            direction.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            //move the character
            rb.velocity = (direction * speed * Time.deltaTime * 50);
            // change idle to run anim
            animator.SetFloat("player_speed", Mathf.Max(Mathf.Abs(rb.velocity.y), Mathf.Abs(rb.velocity.x)));
        }
        CheckDash();
        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            playerController.KillPlayer();
        }
    }
}