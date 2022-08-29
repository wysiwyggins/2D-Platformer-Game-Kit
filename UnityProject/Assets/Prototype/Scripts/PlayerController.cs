using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public AudioClip jumpSound;
    public AudioClip bumpSound;

    const float speed = 10;
    const float jumpSpeed = 25;
    const float gravityScale = 8;
    const float airControl = 3;
    const float groundControl = 10;
    const float maxSpeed = 30;

    bool landToggle = true;
    bool hitToggle = true;
    LayerMask groundMask = 1;
    float horizontalInput, velocityX;
    Vector2 velocity, groundCastPosition, wallCastPosition;
    RaycastHit2D grounded;
    RaycastHit2D walled;
    Rigidbody2D rb;
    PhysicsMaterial2D mat;
    SpriteRenderer spriteRenderer;
    AudioSource audioSource;
    TrailRenderer trail;

    Animator anim;
    int speedHash;
    int jumpHash;

    void Awake()
    {

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        mat = new PhysicsMaterial2D();
        mat.friction = 0;
        rb.sharedMaterial = mat;
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        trail = GetComponent<TrailRenderer>();
        anim = GetComponent<Animator>();
        speedHash = Animator.StringToHash("Speed");
        jumpHash = Animator.StringToHash("Jump");

#if !UNITY_EDITOR
        if (trail != null)
        {
            trail.enabled = false;
        }
#endif
    }

    void Start()
    {
        GameManager.RegisterPlayer(this);
    }

    void Update()
    {
        // Update Animator
        anim.SetFloat(speedHash, Mathf.Abs(rb.velocity.x / speed));
        anim.SetBool(jumpHash, !grounded);

        // Flip sprite
        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            spriteRenderer.flipX = horizontalInput < 0f;
        }
    }

    void FixedUpdate()
    {
        var fixedTime = Time.fixedDeltaTime;

        // Wall/obstacle check
        wallCastPosition = Mathf.Abs(horizontalInput) < 1 ? Vector2.Lerp(wallCastPosition, rb.position + new Vector2(horizontalInput * 0.4f, 0), fixedTime * 10) : rb.position + new Vector2(horizontalInput * 0.4f, 0);
        wallCastPosition.y = rb.position.y;
        walled = Physics2D.CircleCast(wallCastPosition, 0.3f, Vector2.zero, 0, groundMask.value);

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

        // Ground check
        groundCastPosition = Vector2.Lerp(groundCastPosition, rb.position + Vector2.up * -0.4f, fixedTime * 20);
        groundCastPosition.y = Mathf.Min(groundCastPosition.y, rb.position.y - 0.4f);
        grounded = Physics2D.CircleCast(groundCastPosition, 0.4f, Vector2.zero, 0, groundMask.value);

        // Control and grounding
        var control = airControl;
        if (grounded)
        {
            control = groundControl;

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
        velocityX = Mathf.Lerp(velocityX, horizontalInput, fixedTime * control);
        velocity.Set(velocityX * speed, rb.velocity.y);

        // Update velocity
        rb.velocity = velocity;

        // Speed Limit
        //rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
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

    public void Jump()
    {
        if (grounded || walled)
        {
            // Jump vector - combine jump speed with half current vertical velocity
            var velY = rb.velocity.y;
            rb.velocity = rb.velocity * Vector2.right;
            rb.velocity += Vector2.up * (jumpSpeed + Mathf.Max(velY / 2, 0));
            //rb.velocity += Vector2.up * (jumpSpeed + Mathf.Max(Mathf.Pow(rb.velocity.y, 0.5f), 0));

            // Play jump sound
            PlayAudioClip(jumpSound, rb.velocity.y);
        }
    }

    public void HorizontalInput(float value)
    {
        horizontalInput = value;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCastPosition, 0.4f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(wallCastPosition, 0.3f);
    }
}
