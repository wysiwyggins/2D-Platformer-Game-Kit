using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public AudioClip jumpSound;
    public AudioClip bumpSound;
    
    public float HorizontalInput { get; set; }

    const float speed = 10;
    const float jumpSpeed = 25;
    const float gravityScale = 8;
    const float airControl = 3;
    const float groundControl = 10;
    const float maxSpeed = 30;

    bool landToggle = true;
    bool hitToggle = true;
    LayerMask groundMask = 1;
    float velocityX;
    Vector2 velocity;
    RaycastHit2D grounded;
    RaycastHit2D walled;
    Rigidbody2D rb;
    PhysicsMaterial2D mat;
    SpriteRenderer spriteRenderer;
    AudioSource audioSource;
    TrailRenderer trail;

    Animator anim;

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
        // Flip sprite
        if (Mathf.Abs(HorizontalInput) > 0.01f)
        {
            spriteRenderer.flipX = HorizontalInput < 0f;
        }

        // Reset
        if (transform.position.y < -50)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void FixedUpdate()
    {
        // Wall/obstacle check
        walled = Physics2D.CircleCast(rb.position + new Vector2(HorizontalInput * 0.4f, 0), 0.3f, Vector2.zero, 0, groundMask.value);

        if (walled)
        {
            // Reduce horizontal input value if pushing object or against a wall
            HorizontalInput *= 0.5f;

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
        grounded = Physics2D.CircleCast(rb.position + Vector2.up * -0.4f, 0.4f, Vector2.zero, 0, groundMask.value);

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
        velocityX = Mathf.Lerp(velocityX, HorizontalInput, Time.fixedDeltaTime * control);
        velocity.Set(velocityX * speed, rb.velocity.y);

        // Update velocity
        rb.velocity = velocity;

        // Speed Limit
        //rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * -0.4f, 0.4f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + new Vector3(HorizontalInput * 0.4f, 0f, 0), 0.3f);
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
            rb.velocity += Vector2.up * (jumpSpeed + rb.velocity.y / 2 - rb.velocity.y);

            // Play jump sound
            PlayAudioClip(jumpSound, rb.velocity.y);
        }
    }
}
