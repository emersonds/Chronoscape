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

    [SerializeField]
    private float accelSpeed = 1;

    [SerializeField]
    private float decelSpeed = 1;

    [SerializeField]
    private float turnAroundSpeed = 1;

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
    }

    private void FixedUpdate()
    {
        if(!snappyMovement)
        {
            // Current implementation makes physics system do all the work via friction- requires changing player mass/ adding a physics material to plqayer to tweak how slippery player is 
            if (moveVec.x != 0)
            {
                if (rb.velocity.x != 0)
                {
                    if (GetScalar(rb.velocity.x) == GetScalar(moveVec.x))
                    {
                        if (Mathf.Abs(rb.velocity.x) < moveSpeed)
                            rb.velocity = new Vector3(rb.velocity.x + (accelSpeed * GetScalar(rb.velocity.x)), 0, rb.velocity.z);
                        else
                            rb.velocity = new Vector3(moveSpeed * GetScalar(rb.velocity.x), 0, rb.velocity.z);
                    }
                    else
                        rb.velocity = new Vector3(rb.velocity.x + (turnAroundSpeed * GetScalar(moveVec.x)), 0, rb.velocity.z);
                }
                else
                    rb.velocity = new Vector3(rb.velocity.x + (accelSpeed * GetScalar(moveVec.x)), 0, rb.velocity.z);
            }
            else
            {
                if(Mathf.Abs(rb.velocity.x) < 1)
                    rb.velocity = new Vector3(0, 0, rb.velocity.z);
                else
                    rb.velocity = new Vector3(rb.velocity.x - (decelSpeed * GetScalar(rb.velocity.x)), 0, rb.velocity.z);
            }

            if (moveVec.z != 0)
            {
                if (rb.velocity.z != 0)
                {
                    if (GetScalar(rb.velocity.z) == GetScalar(moveVec.z))
                    {
                        if (Mathf.Abs(rb.velocity.z) < moveSpeed)
                            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z + (accelSpeed * GetScalar(rb.velocity.z)));
                        else
                            rb.velocity = new Vector3(rb.velocity.x, 0, moveSpeed * GetScalar(rb.velocity.z));
                    }
                    else
                        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z + (turnAroundSpeed* GetScalar(moveVec.z)));
                }
                else
                    rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z + (accelSpeed * GetScalar(moveVec.z)));
            }
            else
            {
                if (Mathf.Abs(rb.velocity.z) < 1)
                    rb.velocity = new Vector3(rb.velocity.x, 0, 0);
                else
                    rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z - (decelSpeed * GetScalar(rb.velocity.z)));
            }
        }
    }

    private float GetScalar(float x)
    {
        return Mathf.Abs(x) / x;
    }
}
