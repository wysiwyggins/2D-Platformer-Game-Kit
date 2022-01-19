using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    const bool hideCursor = true;
    const float speed = 10;
    const float jumpSpeed = 25;
    const float gravityScale = 8;
    const float airControl = 3;
    const float groundControl = 10;
    const float maxSpeed = 40;

    float h, velocityX, lerp;
    Vector2 velocity;
    RaycastHit2D groundHit;
    Rigidbody2D rb;
    PhysicsMaterial2D mat;
    LayerMask mask = 1;
    SpriteRenderer spriteRenderer;
    WaitForFixedUpdate waitForFixed;

    Animator anim;

    void Awake()
    {
        Time.fixedDeltaTime = 1 / 100f;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        mat = new PhysicsMaterial2D();
        mat.friction = 0;
        rb.sharedMaterial = mat;
        spriteRenderer = GetComponent<SpriteRenderer>();
        Cursor.visible = !hideCursor;
    }

    void Update()
    {
        // input
        h = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            if (groundHit)
            {
                rb.velocity += Vector2.up * jumpSpeed;
            }
        }

        // flip sprite
        if (Mathf.Abs(h) > 0.01f)
        {
            spriteRenderer.flipX = h < 0f;
        }

        // reset
        if (transform.position.y < -50)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void FixedUpdate()
    {
        groundHit = Physics2D.CircleCast(rb.position, 0.6f, Vector2.zero, 0, mask.value);

        lerp = airControl; // air control
        if (groundHit) // grounded
        {
            lerp = groundControl;
        }

        // reduce horizontal input value if pushing object or against a wall
        if (Physics2D.CircleCast(rb.position + Vector2.right * h * 0.5f, 0.25f, Vector2.zero, 0, mask.value))
        {
            h *= 0.2f;
        }

        velocityX = Mathf.Lerp(velocityX, h, Time.fixedDeltaTime * lerp);
        velocity.Set(velocityX * speed, rb.velocity.y);
        rb.velocity = velocity;
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);

        //if (groundHit)
        //{
        //    Rigidbody2D r = groundHit.collider.GetComponentInParent<Rigidbody2D>();
        //    if (r != null) { rb.AddForceAtPosition(r.velocity * 0.5f, groundHit.point, ForceMode2D.Force); } // stick to stuffs
        //}
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.6f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.right * h * 0.5f, 0.25f);

        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(new Vector3(0, -300, 0), new Vector3(5000, 500, 0));
    }
}
