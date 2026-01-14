using System.Collections;
using UnityEngine;

public class PlayerControl_sc : MonoBehaviour
{
    Rigidbody rb;

    public TMPro.TextMeshProUGUI powerText;

    public AudioSource sfxSource;
    public AudioClip actionSound;
    public AudioClip powerUpSound;

    public float hareketHizi;
    Vector3 hareketYonu;

    public float horizontal;
    public float vertical;

    public float dashGucu = 5f;

    public float shoulderPushGucu = 5f;

    public float ziplamaGucu = 5f;
    public float slamYaricapi = 3f;
    public float slamItmeGucu = 100f;

    bool isJump = false;

    public bool powerActive = false;
    public float powerMultiplier = 2f;
    public float powerDuration = 3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        hareketYonu = new Vector3(horizontal, 0, vertical).normalized;

        rb.AddForce(hareketYonu * hareketHizi * Time.fixedDeltaTime);

        if(isJump && IsGrounded())
        {
            SlamShockwave();
            isJump = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(hareketYonu.sqrMagnitude > 0.01f)
        {
            transform.forward = hareketYonu;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Dash();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ShoulderPush();
        }

        if(Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            SlamJump();
        }

        if (Input.GetKeyDown(KeyCode.E) && !powerActive)
        {
            StartCoroutine(PowerStanceRoutine());
        }

        // player zemin dýþýna çýkýp düþerse de kaybeder
        if (transform.position.y < -5f && !Object.FindFirstObjectByType<GameManager_sc>().gameEnded)
        {
            Object.FindFirstObjectByType<GameManager_sc>().PlayerLost();
        }

    }

    void Dash()
    {
        rb.AddForce(transform.forward * dashGucu, ForceMode.Impulse);
        StartCoroutine(PlayLimitedSoundRoutine(actionSound, 0.2f));
    }

    void ShoulderPush()
    {
        rb.AddForce(transform.right * shoulderPushGucu, ForceMode.Impulse);
        StartCoroutine(PlayLimitedSoundRoutine(actionSound, 0.2f));
    }

    void SlamJump()
    {
        rb.AddForce(Vector3.up * ziplamaGucu, ForceMode.Impulse);
        StartCoroutine(PlayLimitedSoundRoutine(actionSound, 0.2f));
        isJump = true;
    }

    void SlamShockwave()
    {
        Collider[] carpilanlar = Physics.OverlapSphere(transform.position, slamYaricapi);
        
        foreach(Collider c in carpilanlar)
        {
            if (c.CompareTag("Rakip"))
            {
                Rigidbody rakipRb = c.GetComponent<Rigidbody>();
                if(rakipRb != null)
                {
                    Vector3 itme = (c.transform.position - transform.position).normalized;
                    rakipRb.AddForce(itme * slamItmeGucu, ForceMode.Impulse);
                }
            }
        }
    }

    // zeminde mi kontrolü yapýlýr (tag: ground)
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    // 3 saniye boyunca güçlenme coroutine
    IEnumerator PowerStanceRoutine()
    {
        powerActive = true;

        powerText.gameObject.SetActive(true);
        powerText.text = "POWER MODE!";

        sfxSource.PlayOneShot(powerUpSound);

        dashGucu *= powerMultiplier;
        shoulderPushGucu *= powerMultiplier;
        slamItmeGucu *= powerMultiplier;

        yield return new WaitForSeconds(powerDuration);

        dashGucu /= powerMultiplier;
        shoulderPushGucu /= powerMultiplier;
        slamItmeGucu /= powerMultiplier;

        powerActive = false;

        powerText.gameObject.SetActive(false);
    }

    // uzun ses efektleri kýsa olsun diye coroutine
    IEnumerator PlayLimitedSoundRoutine(AudioClip clip, float duration)
    {
        sfxSource.PlayOneShot(clip);
        yield return new WaitForSeconds(duration);
        sfxSource.Stop();
    }

    // player zemine çarparsa kaybeder ( tag: ground)
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Object.FindFirstObjectByType<GameManager_sc>().PlayerLost();

            Destroy(gameObject, 0.5f);
        }
    }
}
