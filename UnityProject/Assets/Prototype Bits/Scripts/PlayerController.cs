using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public AudioClip jumpSound;
    public AudioClip bumpSound;

    [Header("Movement Properties")]
    [Range(1, 40)]
    public float speed = 10;
    [Range(1, 60)]
    public float jumpSpeed = 25;
    [Range(1, 20)]
    public float gravityScale = 8;
    [Range(1, 20)]
    public float airControl = 3;
    [Range(1, 20)]
    public float groundControl = 10;
    [Range(1, 100)]
    public float maxSpeed = 30;

    private bool landToggle = true;
    private bool hitToggle = true;

    LayerMask groundMask = 1;
    float horizontalInput, velocityX;
    Vector2 velocity, groundCastPosition, wallCastPosition;
    Vector3 lastPosition;
    RaycastHit2D grounded;
    RaycastHit2D walled;
    Rigidbody2D rb;
    Collider2D coll;
    PhysicsMaterial2D mat;
    SpriteRenderer spriteRenderer;
    AudioSource audioSource;
    ParticleSystem particle;
    TrailRenderer trail;
    RaycastHit2D[] hits = new RaycastHit2D[1];
    ContactFilter2D filter = new ContactFilter2D();

    Animator anim;
    int horizontalSpeedHash, verticalSpeedHash, jumpHash;

    public void SaveDefaultValues()
    {
        PlayerPrefs.SetFloat("DefaultSpeed", speed);
        PlayerPrefs.SetFloat("DefaultJumpSpeed", jumpSpeed);
        PlayerPrefs.SetFloat("DefaultGravityScale", gravityScale);
        PlayerPrefs.SetFloat("DefaultAirControl", airControl);
        PlayerPrefs.SetFloat("DefaultGroundControl", groundControl);
        PlayerPrefs.SetFloat("DefaultMaxSpeed", maxSpeed);
    }
    public void LoadDefaultValues()
    {
        speed = PlayerPrefs.GetFloat("DefaultSpeed", speed);
        jumpSpeed = PlayerPrefs.GetFloat("DefaultJumpSpeed", jumpSpeed);
        gravityScale = PlayerPrefs.GetFloat("DefaultGravityScale", gravityScale);
        airControl = PlayerPrefs.GetFloat("DefaultAirControl", airControl);
        groundControl = PlayerPrefs.GetFloat("DefaultGroundControl", groundControl);
        maxSpeed = PlayerPrefs.GetFloat("DefaultMaxSpeed", maxSpeed);
    }

    void Awake()
    {
        coll = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        mat = new PhysicsMaterial2D();
        mat.friction = 0;
        rb.sharedMaterial = mat;
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        particle = GetComponent<ParticleSystem>();
        trail = GetComponent<TrailRenderer>();
        anim = GetComponent<Animator>();
        horizontalSpeedHash = Animator.StringToHash("HorizontalSpeed");
        verticalSpeedHash = Animator.StringToHash("VerticalSpeed");
        jumpHash = Animator.StringToHash("Jump");
        filter.useTriggers = true;

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
        InvokeRepeating(nameof(Clock), 2, 2);
        lastPosition = rb.position;
    }

    void Update()
    {
        // Update Animator
        anim.SetFloat(horizontalSpeedHash, Mathf.Abs(rb.velocity.x / speed));
        anim.SetFloat(verticalSpeedHash, Mathf.Abs(rb.velocity.y / jumpSpeed));
        anim.SetBool(jumpHash, !grounded);

        // Flip sprite
        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            spriteRenderer.flipX = horizontalInput < 0f;
        }
    }

    void FixedUpdate()
    {
        rb.gravityScale = gravityScale;
        var fixedTime = Time.fixedDeltaTime;

        // Wall/obstacle check
        wallCastPosition = Mathf.Abs(horizontalInput) < 1 ? Vector2.Lerp(wallCastPosition, rb.position + new Vector2(horizontalInput * 0.4f, 0), fixedTime * 20) : rb.position + new Vector2(horizontalInput * 0.4f, 0);
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
                particle.Emit(5);
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

    void Clock()
    {
        if (coll.Raycast(Vector2.down, filter, hits, 5) > 0)
        {
            lastPosition = hits[0].point;
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

    public void ResetPosition()
    {
        rb.velocity = Vector2.zero;
        transform.position = lastPosition + Vector3.up * 0.25f;
    }

    void OnDestroy()
    {
        CancelInvoke();
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCastPosition, 0.4f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(wallCastPosition, 0.3f);
    }
}
