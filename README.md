# 🥇 Güreş Arenası – AI Destekli WebGL Oyun

Bu proje, Unity kullanılarak geliştirilmiş, **Yapay Sinir Ağı (Single Layer Perceptron)** tabanlı yapay zeka entegrasyonuna sahip bir 3D Arena Güreş oyunudur.

Oyuncu, klavye kontrolleriyle karakterini yönetirken; Rakip (AI), harici bir genetik veri dosyasından (`best_genes.json`) okuduğu ağırlıklara göre karar verir.

🎮 **Oyunu tarayıcıda hemen oyna:** 👉 https://aynuradibelli.itch.io/gre-arenas

---

## 🧠 Yapay Zeka Mimarisi (AI Architecture)

Projede **Kural Tabanlı (Rule-Based)** yapay zeka yerine, **Veri Odaklı (Data-Driven)** bir karar mekanizması kullanılmıştır.

### 1. Perceptron Mantığı
Rakip ajan, çevresinden gelen verileri (Input) alır, eğitilmiş ağırlıklarla (Weights) işler ve bir çıktı (Output) üretir.

* **Inputs (Girdiler):** Oyuncuya olan X ve Z mesafesi, Aradaki toplam uzaklık.
* **Weights (Ağırlıklar):** `best_genes.json` dosyasından okunan genetik katsayılar.
* **Activation (Karar):** Girdiler ve ağırlıkların matris çarpımı sonucu çıkan değer, belirli bir eşiği (Threshold) geçerse ajan saldırı (Charge/Push) kararı alır.

### 2. Eğitilebilir Yapı & Dosya Yönetimi
* **Rastgele Mod (Aptal Ajan):** Eğer oyun menüsünden yapay zeka yüklenmezse, ajan rastgele ağırlıklarla (random weights) başlatılır. Bu durumda ajan titrer, kararsız davranır veya saçmalar.
* **Eğitilmiş Mod (Akıllı Ajan):** Menüdeki **"Yapay Zekayı Yükle"** butonuna basıldığında, proje dizinindeki (`StreamingAssets`) JSON dosyası okunur. Optimize edilmiş ağırlıklar ajana yüklenir ve ajan mantıklı (defansif/ofansif) kararlar almaya başlar.

⚠️ **Önemli Not:** Ajanın zekası kodun içine gömülü (hardcoded) değildir. `best_genes.json` dosyasındaki sayılar değiştirildiğinde ajanın karakteri (agresifliği/hızı) tamamen değişir.

---

## 📌 Proje Özeti

- **Platform:** WebGL / PC
- **Motor:** Unity 2022/2023
- **Dil:** C#
- **Kazanma Koşulu:** Arena dışına düşen oyuncu kaybeder; rakip düşerse oyuncu kazanır.

---

## 🎮 Kontroller (Oyuncu)

| Tuş | Aksiyon |
|-----|---------|
| **Ok Tuşları** | Hareket |
| **Shift** | Dash (Hızlı Kaçış) |
| **Q** | Shoulder Push (İtme) |
| **Space** | Slam Jump (Zıplama) |
| **E** | Power Mode (Güçlenme) |

---

## 📁 Dosya Yapısı ve Kurulum

Proje Github deposunda gereksiz dosyalardan arındırılmış (Clean Repo) haldedir.

* **`Assets/StreamingAssets/best_genes.json`:** Yapay zekanın beyin dosyasıdır. WebGL build alındığında verilerin kaybolmaması için bu özel klasörde tutulmaktadır.
* **`best_genes.json` (Ana Dizin):** İnceleme kolaylığı açısından proje kök dizinine de bir kopyası eklenmiştir.

---

## 🔊 Ses ve Müzik

Ana menüde Unity AudioMixer ile entegre çalışan iki ses kanalı bulunur:
- 🎵 **Müzik Sesi**
- 🔈 **Efekt Sesi**

---

## 📜 Lisans ve Teslim Notları

Bu proje, "Oyun Programlama ve Yapay Zeka" dersi final projesi kapsamında geliştirilmiştir.
* **AI Yöntemi:** Single Layer Perceptron / Heuristic Optimization
* **Veri Okuma:** UnityWebRequest (JSON Parsing)