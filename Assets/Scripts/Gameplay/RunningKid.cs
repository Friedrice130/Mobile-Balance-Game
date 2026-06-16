using UnityEngine;
using System.Collections;

public class RunningKid : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform targetPoint;
    public float moveSpeed = 3f;
    public bool useRunAnimation = false;
    public float turnaroundDelay = 0.5f;

    [Header("Audio Settings")]
    public SoundData impactSound;
    public SoundData giggleSound;
    public float giggleInterval = 1.5f;
    public SoundData walkFootstepSound;
    public float walkStepInterval = 0.5f;
    public SoundData runFootstepSound;
    public float runStepInterval = 0.25f;

    private Animator anim;
    private Rigidbody rb;
    private AudioSource voiceSource;

    private bool isMoving = false;
    private bool isPaused = false;
    private Vector3 pointA;
    private Vector3 pointB;
    private Vector3 currentDestination;

    private float stepTimer = 0f;
    private float giggleTimer = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        voiceSource = gameObject.AddComponent<AudioSource>();
        voiceSource.spatialBlend = 1f;
        if (AudioManager.Instance != null && AudioManager.Instance.sfxMixerGroup != null)
        {
            voiceSource.outputAudioMixerGroup = AudioManager.Instance.sfxMixerGroup;
        }

        pointA = transform.position;
        if (targetPoint != null)
        {
            pointB = targetPoint.position;
            currentDestination = pointB;
        }
    }

    void Update()
    {
        if (isMoving && !isPaused)
        {
            // Footstep Logic
            stepTimer -= Time.deltaTime;
            
            if (stepTimer <= 0f)
            {
                SoundData currentStepSound = useRunAnimation ? runFootstepSound : walkFootstepSound;
                float currentInterval = useRunAnimation ? runStepInterval : walkStepInterval;

                if (AudioManager.Instance != null && currentStepSound != null)
                {
                    AudioManager.Instance.PlayAtPoint(currentStepSound, transform.position);
                }
                
                stepTimer = currentInterval;
            }

            // Giggle Logic
            if (useRunAnimation && giggleSound != null)
            {
                if (!voiceSource.isPlaying)
                {
                    giggleTimer -= Time.deltaTime;
                    
                    if (giggleTimer <= 0f)
                    {
                        AudioClip clipToPlay = giggleSound.GetRandomClip();
                        
                        if (clipToPlay != null)
                        {
                            voiceSource.clip = clipToPlay;
                            voiceSource.volume = Random.Range(giggleSound.minVolume, giggleSound.maxVolume);
                            voiceSource.pitch = Random.Range(giggleSound.minPitch, giggleSound.maxPitch);
                            voiceSource.minDistance = giggleSound.minDistance;
                            voiceSource.maxDistance = giggleSound.maxDistance;
                            
                            voiceSource.Play();
                        }
                        
                        giggleTimer = giggleInterval;
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (isMoving && !isPaused && targetPoint != null)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb.position, currentDestination, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);

            Vector3 direction = (currentDestination - rb.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, Time.fixedDeltaTime * 15f));
            }

            if (Vector3.Distance(rb.position, currentDestination) < 0.1f)
            {
                StartCoroutine(TurnAroundRoutine());
            }
        }
    }

    public void StartMoving()
    {
        if (isMoving) return;
        
        isMoving = true;
        
        if (anim != null)
        {
            if (useRunAnimation) anim.CrossFade("Run", 0.2f);
            else anim.CrossFade("Walk", 0.2f);
        }

        giggleTimer = 0f;
        stepTimer = 0.1f;
    }

    private IEnumerator TurnAroundRoutine()
    {
        isPaused = true;

        if (anim != null) anim.CrossFade("Idle", 0.2f);

        yield return new WaitForSeconds(turnaroundDelay);

        // Swap destination
        if (currentDestination == pointA) currentDestination = pointB;
        else currentDestination = pointA;

        if (anim != null)
        {
            if (useRunAnimation) anim.CrossFade("Run", 0.2f);
            else anim.CrossFade("Walk", 0.2f);
        }

        isPaused = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Item"))
        {
            isMoving = false;

            if (anim != null) anim.CrossFade("Idle", 0.1f);

            if (voiceSource != null && voiceSource.isPlaying)
            {
                voiceSource.Stop();
            }

            if (AudioManager.Instance != null && impactSound != null)
            {
                AudioManager.Instance.PlayAtPoint(impactSound, transform.position);
            }
        }
    }
}