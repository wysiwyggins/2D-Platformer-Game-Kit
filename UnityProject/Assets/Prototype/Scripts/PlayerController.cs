using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    // Settings
    float speed = 15;
    float jumpSpeed = 25;
    float gravityScale = 8;
    float airControl = 3;
    float groundControl = 10;
    float maxSpeed = 40;
    float hitRadius = 0.6f;
    float groundCheckOffset = 0.1f;
    LayerMask collisionMask = 1;

    // Vars
    bool bInputJump = true;
    float velocityX;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    new Transform transform;

    void Awake()
    {
        transform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Cursor.visible = false;
        rb.gravityScale = gravityScale;
        Time.fixedDeltaTime = 1 / 125; // Set physics tick rate
        var pm = new PhysicsMaterial2D();
        pm.friction = 0;
        rb.sharedMaterial = pm;
    }


    void FixedUpdate()
    {
        // Ground check
        RaycastHit2D groundHit = Physics2D.CircleCast(rb.position - Vector2.up * groundCheckOffset, hitRadius, Vector2.zero, 0, collisionMask.value);

        // Input
        var horizontalInput = 0f;
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed) { horizontalInput = -1; }
        if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed) { horizontalInput = 1; }

        if (Keyboard.current.spaceKey.isPressed)
        {
            if (bInputJump && groundHit)
            {
                rb.velocity += Vector2.up * jumpSpeed;
            }
            bInputJump = false;
        }
        else
        {
            bInputJump = true;
        }

        // Sprite flip
        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            spriteRenderer.flipX = horizontalInput < 0f;
        }

        // Velocity
        float lerp = groundHit ? groundControl : airControl;
        velocityX = Mathf.Lerp(velocityX, horizontalInput, Time.fixedDeltaTime * lerp);
        rb.velocity = new Vector2(velocityX * speed, rb.velocity.y); ;

        // Speed limit
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);

        // Reset level if player falls in to "deathbox"
        if (transform.position.y < -50)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }


        // Experimetnal strafe
        //// velocity += acceleration - friction*velocity // Stokes friction model
        //v = motionVector;// Vector3.Lerp(v, motionVector, velocityLerp * fixedDeltaTime);
        //var velocity = v - 0.6f * _rigidbody.velocity;
        //velocity += _rigidbody.velocity * (deltaY * 3); // strafe
        //velocity.y = 0;
        //_rigidbody.velocity += velocity;
    }

    void OnDrawGizmos()
    {
        transform = GetComponent<Transform>();
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position - Vector3.up * groundCheckOffset, hitRadius);

        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(new Vector3(0, -150, 0), new Vector3(5000, 200, 0));
    }
}
