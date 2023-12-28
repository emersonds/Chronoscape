using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simplistic player mover for the time being; will eventually be the root class ALL playable characters inherit from & modify for ALL player functionality
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField, Tooltip("How fast the player should move at max speed")]
    private float moveSpeed = 5;

    [SerializeField, Tooltip("Whether player velocity changes should be instant or have some acceleration/ deceleration")]
    private bool snappyMovement = false;

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
        if(snappyMovement)
            rb.velocity = moveVec;

        // If we want the player to decelerate while not holding in a direction
        else
        {
            // Current implementation makes physics system do all the work via friction- requires changing player mass/ adding a physics material to plqayer to tweak how slippery player is 
            if (moveVec.x != 0)
                rb.velocity = new Vector3(moveVec.x, 0, rb.velocity.z);
            if (moveVec.z != 0)
                rb.velocity = new Vector3(rb.velocity.x, 0, moveVec.z);

            // TODO: IF we want to flesh this out more, we can apply an opposite force to whatever direction has no current input. We can also apply even more of this force if the player
            // suddenly changes direction. The idea here is if the player just releases the controls, they decelerate slowish. However, if they were going forward & start holding back, they should
            // decelerate quickly (~2x speed they would decelerate w/o holding any direction), and once velocity is 0 they should start moving in the opposite direction
            // We should also add in an acceleration factor just to make it more smooth so they don't reach max speed instantly (since they're not slowing down instantly)
        }
    }
}
