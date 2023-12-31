using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simplistic player mover for the time being; will eventually be the root class ALL playable characters inherit from & modify for ALL player functionality
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField, Tooltip("How fast the player should move at max speed")]
    private float moveSpeed = 5f;

    [SerializeField, Tooltip("How fast the player should rotate")]
    private float rotateSpeed = 30f;

    [SerializeField, Tooltip("Whether player velocity changes should be instant or have some acceleration/ deceleration")]
    private bool snappyMovement = false;

    [SerializeField]
    private float accelSpeed = 1f;

    [SerializeField]
    private float decelSpeed = 1f;

    [SerializeField]
    private float turnAroundSpeed = 1f;

    // The player's current movement vector
    Vector3 moveVec = Vector3.zero;

    // The player's Rigidbody component
    Rigidbody rb = null;


    // Start is called before the first frame update
    void Start()
    {
        // Get Rigidbody component
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Sets a Vector3 using the current value of Horizontal & Vertical inputs (defined in old input manager) & multiplies it by scalar moveSpeed
        moveVec = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")) * moveSpeed;

        // If player should start & stop instantly, velocity is 1:1 w/ player's input
        if (snappyMovement)
            rb.velocity = moveVec;

        if (moveVec != Vector3.zero)
        {
            Quaternion toRot = Quaternion.LookRotation(rb.velocity);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRot, rotateSpeed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        // Move the player
        if (!snappyMovement)
            rb.velocity = new Vector3(UpdateAxisVelocity(rb.velocity.x, moveVec.x), 0, UpdateAxisVelocity(rb.velocity.z, moveVec.z));
    }

    /// <summary>
    /// Updates player velocity on a per-axis bases
    /// </summary>
    /// <param name="playerVelocity">The player's current velocity in a direction (x or z)</param>
    /// <param name="moveInput">The player's current input direction (x or z)</param>
    /// <returns>The velocity component (x or z)</returns>
    private float UpdateAxisVelocity(float playerVelocity, float moveInput) // Original movement code by me; simplified into method via ChatGPT
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
                    playerVelocity = Mathf.Abs(playerVelocity) < moveSpeed ? playerVelocity + (accelSpeed * GetScalar(playerVelocity)) : moveSpeed * GetScalar(playerVelocity);
                else
                    // If we are trying to move in the direction opposite of movement, quickly decelerate
                    playerVelocity += turnAroundSpeed * GetScalar(moveInput);
            }
            // If the player is NOT moving, get them moving a bit (avoids divide by 0 error)
            else
                playerVelocity += accelSpeed * GetScalar(moveInput);
        }
        // If there is no input from player, slowly decelerate until we're basically not moving & set velocity in that axis to 0
        else
            playerVelocity = Mathf.Abs(playerVelocity) < 1 ? 0 : playerVelocity - (decelSpeed * GetScalar(playerVelocity));

        // Returns the new velocity component
        return playerVelocity;
    }

    /// <summary>
    /// Get the scaler value of a float
    /// </summary>
    /// <param name="x">The float to get the scalar of</param>
    /// <returns>The scalar value of x</returns>
    private float GetScalar(float x)
    {
        return Mathf.Abs(x) / x;
    }
}
