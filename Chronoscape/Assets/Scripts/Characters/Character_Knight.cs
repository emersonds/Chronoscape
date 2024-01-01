using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Knight : PlayerController
{
    // May be helpful to reference this documentation while designing the characters:
    // https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/object-oriented/polymorphism

    [SerializeField, Tooltip("How much force is applied to the player to dash when moving.")]
    private float movingDashForce = 100f;

    [SerializeField, Tooltip("How much force is applied to the player to dash when not moving.")]
    private float standingDashForce = 100f;

    [SerializeField, Tooltip("How long the player can dash for.")]
    private float dashDuration = 1f;

    [SerializeField, Tooltip("How long the player has to wait until dash is active after use.")]
    private float dashCooldown = 5f;

    // If the player can dash
    private bool canDash = true;

    // If the player is dashing
    private bool isDashing = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void GetInput()
    {
        base.GetInput();
    }

    protected override float UpdateAxisVelocity(float playerVelocity, float moveInput)
    {
        // If there is input from the player
        if (moveInput != 0)
        {
            // If the player character is in motion
            if (playerVelocity != 0)
            {
                // If we are trying to move in the direction of movement
                if (GetScalar(playerVelocity) == GetScalar(moveInput))
                    if (!isDashing)
                        // If we are not at max speed, accelerate. Otherwise, cap player at max speed.
                        playerVelocity = Mathf.Abs(playerVelocity) < MoveSpeed ? playerVelocity + (AccelSpeed * GetScalar(playerVelocity)) : MoveSpeed * GetScalar(playerVelocity);
                    else
                        playerVelocity += (AccelSpeed * GetScalar(playerVelocity));
                else
                    // If we are trying to move in the direction opposite of movement, quickly decelerate
                    playerVelocity += TurnAroundSpeed * GetScalar(moveInput);
            }
            // If the player is NOT moving, get them moving a bit (avoids divide by 0 error)
            else
                playerVelocity += AccelSpeed * GetScalar(moveInput);
        }
        // If there is no input from player, slowly decelerate until we're basically not moving & set velocity in that axis to 0
        else if (moveInput == 0 && !isDashing)
            playerVelocity = Mathf.Abs(playerVelocity) < 1 ? 0 : playerVelocity - (DecelSpeed * GetScalar(playerVelocity));

        // Returns the new velocity component
        return playerVelocity;
    }

    protected override void ActivateBasicAbility()
    {
        if (canDash)
        {
            Vector3 currMoveVec = moveVec;
            StartCoroutine(DashTimer(currMoveVec));
        }
    }


    private IEnumerator DashTimer(Vector3 dir)
    {
        canDash = false;
        isDashing = true;
        Vector3 tempVelocity = rb.velocity;

        if (dir != Vector3.zero)
        {
            animator.SetBool("isDashing", true);
            rb.velocity *= movingDashForce;
            yield return new WaitForSeconds(dashDuration);
        }
        else
        {
            animator.SetBool("isDashing", true);
            rb.AddForce(transform.forward * standingDashForce, ForceMode.Impulse);
            yield return new WaitForSeconds(dashDuration);
        }

        rb.velocity = tempVelocity;
        animator.SetBool("isDashing", false);
        isDashing = false;
        StartCoroutine(DashCooldown());
    }

    private IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
        

    protected override void ActivateUltimateAbility()
    {
        Debug.Log("Ultimate ability cast!");
    }

}
