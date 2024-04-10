    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;




    public class movementBehavior : MonoBehaviour
    {
    #region Variables
    private Rigidbody2D rb;
        private input_manager input;
    bool onwall;
        [Header("Movement Preferences")]
        [SerializeField] float movementSpeed = 10f;
        [SerializeField] float jumpForce = 8f;
    [SerializeField] float jumptime = 0.1f;
   public float jumpTimer = 0f;
         Vector2 movementDir;
   
    public float groundTimer=0f;

        private string groundLayerName = "ground";
        private LayerMask groundLayer;

        [Header("Ground Checking Settings")]
        [SerializeField] float boxWidth = 1f;
        [SerializeField] float boxHeight = 0.1f;
    [SerializeField] float goundedTime = 0.1f;
        [Header("gravity settings")]
    [SerializeField] float originalGravity = 9.8f; // The default gravity value
    [SerializeField] float maxGravity = 20f; // The maximum gravity value
    [SerializeField] float gravityIncreaseAmount = 0.5f; // The amount to increase gravity when falling
    [SerializeField] float gravityTime = .2f;
    SpriteRenderer spriteRef;
        [Header("dash settings")]
        [SerializeField] float dashForce = 5f;
        [SerializeField] float dashCooldown = 2f;
         bool dashIsReady;
        public bool isDashing { get; private set; }

       
        [SerializeField] LayerMask wall_Layer;
        //  bool Sliding;   
        RaycastHit2D Rhit, Lhit;
        [Header("Wall jump settings")]
        [SerializeField] float wallJumpForce = 5f;
        
    [SerializeField] float wallJumpTime = 0.2f;
    private float wallJumpTimer = 0f;
    [SerializeField] float knockBackForce = 15f;

    [Header("wall sliding settings")]
        [SerializeField] float range = 1.2f;
    
        [SerializeField] float slidingSpeed = 1f;
    private bool slideJumpReady;

    #endregion
    private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            input = input_manager.Instance;
            groundLayer = LayerMask.GetMask(groundLayerName);
            rb.freezeRotation = true;
            spriteRef = GetComponent<SpriteRenderer>();
            dashIsReady = true;
        rb.gravityScale = originalGravity;
        }

        private void Update()
        {
        calculateJumpTime();
        if (wallJumpTimer < 0)
        {
            movementDir.x = input.MoveInput.x * movementSpeed;


            rotatePlayer();

        }
        else wallJumpTimer -= Time.deltaTime;
           
        resetSjump();
        isGrounded();
        handleGravity();


    }
        
        private void FixedUpdate()
        {
            rb.velocity = new Vector2(movementDir.x, rb.velocity.y);
            onjump();
            handleDash();
            wallCheck();
        }

        private void onjump()
        {
            if (jumpTimer>0 &&groundTimer>0 )
            {

                rb.velocity = new Vector2(movementDir.x, jumpForce);
            }


        }

        void rotatePlayer()
        {
            if (movementDir.x > 0)
            {
                spriteRef.flipX = false;
            }
            else if (movementDir.x < 0)
            {
                spriteRef.flipX = true;
            }
        }
        void handleDash()
        {
            if (dashIsReady && input.dashTrigger)
            {
                if (input.MoveInput.x == 0) return;
                isDashing = true;
            rb.velocity = new Vector2(rb.velocity.x * dashForce, rb.velocity
                .y);
            
                dashIsReady = false;
                StartCoroutine(delay());
            }
            else isDashing = false;



        }
           void calculateJumpTime(){

        if (input.JumpTrigger) jumpTimer = jumptime;
        else jumpTimer -= Time.deltaTime;
    }
    void handleGravity()
    {
        print(rb.gravityScale);
        rb.gravityScale = Mathf.Clamp(rb.gravityScale, originalGravity, maxGravity);
        if (groundTimer > 0 || onwall) rb.gravityScale = originalGravity;
        else if (groundTimer < 0)
        {
            float current = rb.gravityScale + gravityIncreaseAmount;
            rb.gravityScale = Mathf.Lerp(rb.gravityScale, current, gravityTime);
        }
    }
         IEnumerator delay()
        {

            yield return new WaitForSeconds(dashCooldown);
                dashIsReady = true;
        }


     void isGrounded()
    {
        Vector2 boxPosition = transform.position - new Vector3(0f, boxHeight / 2f, 0f);
        LayerMask groundAndWallLayer = groundLayer | wall_Layer;
        // Only check for colliders within a small vertical range around the character
        Collider2D[] colliders = Physics2D.OverlapBoxAll(boxPosition, new Vector2(boxWidth, boxHeight), 0f, groundAndWallLayer);

        // Check if any colliders are found
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)  // Exclude the character's own collider
            {
                groundTimer = goundedTime;
            }
        }

        print("false");
        groundTimer -= Time.deltaTime;
    }

    void wallCheck()
    {
        onwall = false;
        Rhit = Physics2D.Raycast(transform.position, transform.right, range, wall_Layer);
        Lhit = Physics2D.Raycast(transform.position, -transform.right, range, wall_Layer);

        if (Rhit.collider != null && groundTimer < 0 && input.MoveInput.x == 1)
        {
            onwall = true;
            rb.velocity = new Vector2(rb.velocity.x, -slidingSpeed); // Slide down

            if (input.JumpTrigger && slideJumpReady)
            {
                rb.velocity = new Vector2(rb.velocity.x, -slidingSpeed); // Slide down
                rb.velocity = new Vector2(rb.velocity.x, 0f); // Reset vertical velocity
                wallJumpTimer = wallJumpTime;
                spriteRef.flipX = !spriteRef.flipX;

                rb.velocity = new Vector2(-input.MoveInput.x * knockBackForce, wallJumpForce);

                Debug.Log("Right wall jump");


                slideJumpReady = false;
                onwall = false;
            }
        }
        else if (Lhit.collider != null && groundTimer < 0 && input.MoveInput.x == -1 )
        {
            rb.velocity = new Vector2(rb.velocity.x, -slidingSpeed); // Slide down
            onwall = true;
            if (input.JumpTrigger && slideJumpReady)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f); // Reset vertical velocity
                wallJumpTimer = wallJumpTime;
                spriteRef.flipX = !spriteRef.flipX;

                rb.velocity = new Vector2(-input.MoveInput.x * knockBackForce, wallJumpForce);

                Debug.Log("Right wall jump");


                slideJumpReady = false;
                onwall = false;
            }

        }
        
    }
    void resetSjump()
    {
        if (groundTimer > 0 || Vector2.Distance(transform.position,Rhit.point)>= range * 2|| Vector2.Distance(transform.position, Lhit.point) >= range * 2)  slideJumpReady = true;
       
    }
   /* private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("jumpPad")) {
            rb.velocity = new Vector2(rb.velocity.x,jumpForce *2) ; 
        }
    }*/
}


