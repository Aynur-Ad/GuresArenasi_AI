using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;

// JSON okumak ýcýn yardimci sinif
[System.Serializable]
public class DNA
{
    public float[] genes;
}

public class RakipKontrol_sc : MonoBehaviour
{
    Rigidbody rb;
    Transform player;

    // 7 tane gen: 4 hareket, 3 aksiyon karari icin
    public float[] weights = new float[7];
    float actionCooldown = 0f;
    public float moveSpeed = 500f;
    public float maxVelocity = 5f; // hiz siniri

    void Start()
    {
        /* oyun baslar baslamaz rakip, zeminle(tag: ground) carpisip olmesin diye
        arenada belirlenen optimal nokta etrafýnda rastgele olusturuluyor  */
        float randomX = Random.Range(-2f, 2f);
        float randomY = Random.Range(1f, 2f);
        float randomZ = Random.Range(-2f, 2f);

        transform.position += new Vector3(randomX, randomY, randomZ);

        rb = GetComponent<Rigidbody>();
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        // Hafizada veri varsa ceker, yoksa rastgele hareket eder

        if (AIDataStorage_sc.loadedWeights != null && AIDataStorage_sc.loadedWeights.Length == weights.Length)
        {
            // VARSA ONU KULLAN(Akýllý Mod) -> MenuManager kisminda yapiliyor ve burada kopyaliyoruz
            System.Array.Copy(AIDataStorage_sc.loadedWeights, weights, weights.Length);
        }
        else
        {
            // YOKSA RASTGELE BAÞLA (Aptal Mod)
            RandomizeBrain();
            Debug.Log("Rakip: Veri yok, rastgele (aptal) moddayým.");
        }
    }

    void RandomizeBrain()
    {
        for (int i = 0; i < weights.Length; i++) 
            weights[i] = Random.Range(-1.0f, 1.0f);
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // 1. GIRDILER (Sensorler)
        float dx = player.position.x - transform.position.x;
        float dz = player.position.z - transform.position.z;
        float distance = Vector3.Distance(transform.position, player.position);

        float inputX = dx;
        float inputZ = dz;
        float inputDist = Mathf.Clamp(2f / (distance + 0.1f), 0f, 2f);

        // 2. BEYIN(Karar Mekanizmasi)
        float moveX = (inputX * weights[0]) + (inputZ * weights[1]);
        float moveZ = (inputX * weights[2]) + (inputZ * weights[3]);
        float actionValue = (inputDist * weights[4]) + (inputX * weights[5]) + (inputZ * weights[6]);

        // 3. HAREKET
        Vector3 movement = new Vector3(moveX, 0, moveZ).normalized;

        // hiz limiti asilmadiysa guc uygula, firlayip gitmesin diye
        if (rb.linearVelocity.magnitude < maxVelocity)
        {
            rb.AddForce(movement * moveSpeed * Time.fixedDeltaTime);
        }

        // her zaman player'a yuzunu donsun
        if (movement.sqrMagnitude > 0.1f) transform.forward = movement;

        // 4. AKSIYONLAR (Konsoldan takip edilebilir)
        if (actionCooldown <= 0)
        {
            // Debug koyalim ki hata olursa ne yapmaya calistigini gorelim
            if (actionValue > 0.3f) Debug.Log("AI Dusunuyor... Deger: " + actionValue);

            if (actionValue > 1.5f) // Esik degeri biraz yukseltelim cunku genleri cok artirdik
            {
                Debug.Log("Saldiri: CHARGE!");
                Charge(15f);
                actionCooldown = 3f;
            }
            else if (actionValue > 1.0f)
            {
                Debug.Log("Saldiri: HEAVY MODE!");
                HeavyMode(10f);
                Invoke("ResetMass", 2f);
                actionCooldown = 2.5f;
            }
            else if (actionValue > 0.4f && distance < 2.0f)
            {
                Debug.Log("Saldiri: COUNTER PUSH!");
                Rigidbody playerRb = player.GetComponent<Rigidbody>();
                if (playerRb != null) CounterPush(playerRb, 15f);
                actionCooldown = 1.0f;
            }
        }
        else
        {
            actionCooldown -= Time.fixedDeltaTime;
        }

        // Dusme Kontrolu
        if (transform.position.y < -5f)
        {
            var manager = Object.FindFirstObjectByType<GameManager_sc>();
            if (manager != null && !manager.gameEnded) manager.PlayerWon();
        }
    }

    void ResetMass() {
        rb.mass = 1f; 
    }

    // --- Ilk Asamada Tanimladigim Fonksiyonlarim ---
    public void CounterPush(Rigidbody playerRb, float guc)
    {
        Vector3 itme = (playerRb.transform.position - transform.position).normalized;
        playerRb.AddForce(itme * guc, ForceMode.Impulse);
    }
    public void HeavyMode(float extraMass) { 
        rb.mass += extraMass; 
    }
    public void Charge(float speed) { 
        rb.AddForce(transform.forward * speed, ForceMode.Impulse); 
    }

private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Object.FindFirstObjectByType<GameManager_sc>().PlayerWon();
            Destroy(gameObject, 0.5f);
        }
    }
    
}