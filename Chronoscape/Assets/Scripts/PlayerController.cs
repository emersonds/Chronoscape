using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

/// <summary>
/// A simplistic player mover for the time being; will eventually be the root class ALL playable characters inherit from & modify for ALL player functionality
/// </summary>
public abstract class PlayerController : MonoBehaviour, IDamageable
{
    [field: SerializeField, Tooltip("How fast the player should move at max speed")]
    protected virtual float MoveSpeed { get; private set; }

    [field: SerializeField, Tooltip("How fast the player should rotate")]
    protected virtual float RotateSpeed { get; private set; }

    [field: SerializeField, Tooltip("How fast the player accelerates.")]
    protected virtual float AccelSpeed { get; private set; }

    [field: SerializeField, Tooltip("How fast the player decelerates.")]
    protected virtual float DecelSpeed { get; private set; }

    [field: SerializeField, Tooltip("How fast the player turns around when putting in an opposite input.")]
    protected virtual float TurnAroundSpeed { get; private set; }

    [field: SerializeField, Tooltip("How fast the player attacks. This is a modifier for the attack animation.")]
    protected virtual float AttackSpeed { get; private set; }

    [field: SerializeField, Tooltip("How long until the player can attack again.")]
    protected virtual float AttackCooldown { get; private set; }

    // If the player can attack
    protected bool canAttack = true;

    // If the player is currently attacking (used for animation cancelling)
    protected bool isAttacking = false;

    [field: SerializeField, Tooltip("The player's max health")]
    protected virtual float MaxHealth { get; private set; }

    // Player's current health
    protected float health;

    // The player's current movement vector
    protected Vector3 moveVec = Vector3.zero;

    // The player's Rigidbody component
    protected Rigidbody rb;

    // Player's animator controller
    protected Animator animator;

    // Abstract methods each character may implement
    protected abstract void ActivateBasicAbility();
    protected abstract void ActivateUltimateAbility();
    protected abstract void Attack();


    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Get components
        rb = GetComponentInParent<Rigidbody>();
        animator = GetComponent<Animator>();

        // Start auto attacking
        StartCoroutine(AttackCoroutine());

        health = MaxHealth;
    }


    // Update is called once per frame
    protected virtual void Update()
    {
        GetInput();
    }


    protected virtual void FixedUpdate()
    {
        Move();
    }


    /// <summary>
    /// Takes player input to move and perform actions
    /// </summary>
    protected virtual void GetInput()
    {
        // Sets a Vector3 using the current value of Horizontal & Vertical inputs (defined in old input manager) & multiplies it by scalar moveSpeed
        moveVec = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")) * MoveSpeed;

        // Activates abilities per player input
        if (Input.GetButtonDown("BasicAbility"))
            ActivateBasicAbility();
        if (Input.GetButtonDown("UltimateAbility"))
            ActivateUltimateAbility();
    }

    /// <summary>
    /// Moves the player using 
    /// </summary>
    protected void Move()
    {
        // Move the player
        rb.velocity = new Vector3(UpdateAxisVelocity(rb.velocity.x, moveVec.x), 0, UpdateAxisVelocity(rb.velocity.z, moveVec.z));

        // Rotate player to face movement direction
        if (moveVec != Vector3.zero)
        {
            Quaternion toRot = Quaternion.LookRotation(rb.velocity);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRot, RotateSpeed * Time.deltaTime);
        }

        // Set animations based on player speed
        animator.SetFloat("speed", rb.velocity.magnitude);

    }

    /// <summary>
    /// Updates player velocity on a per-axis bases
    /// </summary>
    /// <param name="playerVelocity">The player's current velocity in a direction (x or z)</param>
    /// <param name="moveInput">The player's current input direction (x or z)</param>
    /// <returns>The velocity component (x or z)</returns>
    protected virtual float UpdateAxisVelocity(float playerVelocity, float moveInput) // Original movement code by me; simplified into method via ChatGPT
    {
        // If there is input from the player
        if (moveInput != 0)
        {
            // If the player character is in motion
            if (playerVelocity != 0)
            {
                // If we are trying to move in the direction of movement
                if (GetScalar(playerVelocity) == GetScalar(moveInput))
                    // If we are not at max speed, accelerate. Otherwise, cap player at max speed.
                    playerVelocity = Mathf.Abs(playerVelocity) < MoveSpeed ? playerVelocity + (AccelSpeed * GetScalar(playerVelocity)) : MoveSpeed * GetScalar(playerVelocity);
                else
                    // If we are trying to move in the direction opposite of movement, quickly decelerate
                    playerVelocity += TurnAroundSpeed * GetScalar(moveInput);
            }
            // If the player is NOT moving, get them moving a bit (avoids divide by 0 error)
            else
                playerVelocity += AccelSpeed * GetScalar(moveInput);
        }
        // If there is no input from player, slowly decelerate until we're basically not moving & set velocity in that axis to 0
        else
            playerVelocity = Mathf.Abs(playerVelocity) < 1 ? 0 : playerVelocity - (DecelSpeed * GetScalar(playerVelocity));

        // Returns the new velocity component
        return playerVelocity;
    }

    /// <summary>
    /// Get the scaler value of a float
    /// </summary>
    /// <param name="x">The float to get the scalar of</param>
    /// <returns>The scalar value of x</returns>
    protected float GetScalar(float x)
    {
        return Mathf.Abs(x) / x;
    }

    public virtual void DoDamage(float damage)
    {
        health -= damage;
    }
    

    /// <summary>
    /// Handles auto attacks by attacking in set intervals.
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator AttackCoroutine()
    {
        // Infinite attack loop
        for (;;)
        {
            // Set attack speed
            animator.SetFloat("attackSpeed", AttackSpeed);

            // Play animation
            animator.Play("Attack", 1);

            // Activate auto attack
            Attack();
            
            yield return new WaitForSeconds(AttackCooldown);
        }
    }
}
