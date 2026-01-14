using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Networking; // Dosya okuma icin sart
using System.Collections;     // Coroutine icin sart
using TMPro;


public class MenuManager_sc : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;

    public TextMeshProUGUI durumText;

    private void Start()
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(0.5f) * 20);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(0.5f) * 20);

        musicSlider.value = 0.5f;
        sfxSlider.value = 0.5f;

        // baslangic yazisi
        if (durumText != null) durumText.text = "Bekliyor...";
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }


    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }

    // Yapay Zekayi Yükle butonunu cagiracak fonksiyon
    public void LoadAIButton()
    {
        if (durumText != null) durumText.text = "Yukleniyor...";
        StartCoroutine(LoadGenesRoutine());
    }

    IEnumerator LoadGenesRoutine()
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "best_genes.json");

        using (UnityWebRequest www = UnityWebRequest.Get(path))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                DNA loadedDNA = JsonUtility.FromJson<DNA>(www.downloadHandler.text);
                if (loadedDNA != null)
                {
                    // Veriyi statik depoya atýyoruz
                    AIDataStorage_sc.loadedWeights = loadedDNA.genes;

                    if (durumText != null)
                    {
                        durumText.text = "AI YUKLENDI! (Hazýr)";
                        durumText.color = Color.green;
                    }

                    Debug.Log("Veri Menüde Yüklendi ve Hafýzaya Alýndý!");
                }
            }
            else
            {
                if (durumText != null)
                {
                    durumText.text = "HATA: Dosya Bulunamadi!";
                    durumText.color = Color.red;
                }

                Debug.LogError("Hata: " + www.error);
            }
        }
    }
}

