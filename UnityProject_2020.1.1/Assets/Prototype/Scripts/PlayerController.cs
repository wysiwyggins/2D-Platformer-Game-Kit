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
    const float maxSpeed = 30;

    public AudioClip jumpSound;
    public AudioClip bumpSound;

    bool landToggle = true;
    bool hitToggle = true;
    LayerMask groundMask = 1;
    float horizontalInput, velocityX;
    Vector2 velocity;
    RaycastHit2D grounded;
    RaycastHit2D walled;
    Rigidbody2D rb;
    PhysicsMaterial2D mat;
    SpriteRenderer spriteRenderer;
    AudioSource audioSource;

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
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Jump if grounded or walled
        if (Input.GetButtonDown("Jump") && (grounded || walled))
        {
            // Jump vector - combine jump speed with half current vertical velocity
            rb.velocity += Vector2.up * (jumpSpeed + rb.velocity.y / 2 - rb.velocity.y);

            // Play jump sound
            PlayAudioClip(jumpSound, rb.velocity.y);
        }

        // Flip sprite
        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            spriteRenderer.flipX = horizontalInput < 0f;
        }

        // Reset
        if (transform.position.y < -50)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void FixedUpdate()
    {
        // Grab horizontal input here, to sync with physics loop
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Assume no input
        var control = 0f;

        // Ground check
        grounded = Physics2D.CircleCast(rb.position + Vector2.up * -0.2f, 0.4f, Vector2.zero, 0, groundMask.value);

        // Wall/obstacle check
        walled = Physics2D.CircleCast(rb.position + new Vector2(horizontalInput * 0.4f, 0), 0.3f, Vector2.zero, 0, groundMask.value);

        if (walled)
        {
            // Reduce horizontal input value if pushing object or against a wall
            horizontalInput *= 0.5f;

            // Play hit sound
            var x = Mathf.Abs(velocity.x);
            if (hitToggle && x > 5)
            {
                PlayAudioClip(bumpSound, x);
            }
            hitToggle = false;
        }
        else
        {
            hitToggle = true;
        }

        // Air control
        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            control = airControl;
        }

        if (grounded)
        {
            control = groundControl; // Ground control

            // Play landing sound
            var y = Mathf.Abs(velocity.y);
            if (landToggle && y > 10)
            {
                PlayAudioClip(bumpSound, y);
            }
            landToggle = false;
        }
        else
        {
            landToggle = true;
        }

        // Calculate horizontal velocity
        velocityX = Mathf.Lerp(velocityX, horizontalInput, Time.fixedDeltaTime * control);
        velocity.Set(velocityX * speed, rb.velocity.y);

        // Update velocity
        rb.velocity = velocity;
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
    }

    void PlayAudioClip(AudioClip clip, float velocity)
    {
        if (audioSource != null && clip != null)
        {
            var volume = Mathf.Clamp01(Mathf.Abs(velocity) / jumpSpeed);
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.volume = volume;
            audioSource.PlayOneShot(clip);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * -0.2f, 0.4f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + new Vector3(horizontalInput * 0.4f, 0f, 0), 0.3f);

        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(new Vector3(0, -300, 0), new Vector3(5000, 500, 0));
    }
}
