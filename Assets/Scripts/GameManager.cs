using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    public Player player;

    public Room room;
    public Enemy boss;
    public int killCount;
    public float playTime;

    public GameObject canvas;

    [Header("[Heart Panel]")]
    public GameObject heartPanel;
    public GameObject fullHeartPrefab;
    public GameObject fullEmptyHeartPrefab;
    public GameObject halfHeartPrefab;

    [Header("[BlankBullet Panel]")]
    public GameObject blankBulletPanel;
    public GameObject blankBulletPrefab;

    public Text keyText;
    public Text coinText;

    [Header("[Weapon Panel]")]
    public GameObject weaponPanel;
    public Image weaponImage;
    public Text ammoText;
    public Text curMagText;

    [Header("[Boss Panel]")]
    public GameObject bossPanel;
    public RectTransform bossHealthImage;

    public GameObject itemPanel;
    public Text itemText;

    public GameObject GameoverPanel;
    public Text timeText;
    public Text resultCoinText;
    public Text killCountText;


    public GameObject titleMenuPanel;
    public GameObject optionsMenuPanel;
    public GameObject gameUI;

    public GameObject pausePanel;

    public Slider[] volumeSliders;

    [Header("[Crosshair]")]
    public GameObject crosshair;
    public Sprite[] crosshairs;
    public Image option_CrosshairImage;
    private int currentCrosshairNum;

    public Image pause_CrosshairIamge;

    public bool Pause = true;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;

            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        volumeSliders[0].value = AudioManager.Instance.masterVolumePercent;
        volumeSliders[1].value = AudioManager.Instance.sfxVolumePercent;
        volumeSliders[2].value = AudioManager.Instance.masterVolumePercent;
        volumeSliders[3].value = AudioManager.Instance.sfxVolumePercent;
        TitleMenu();

        killCount = 0;

        //crosshair 초기화
        currentCrosshairNum = PlayerPrefs.GetInt("crosshair");
        option_CrosshairImage.sprite = crosshairs[currentCrosshairNum];
        pause_CrosshairIamge.sprite = crosshairs[currentCrosshairNum];
        crosshair.GetComponent<SpriteRenderer>().sprite = crosshairs[currentCrosshairNum];

        //panel 초기화
        titleMenuPanel.SetActive(true);
        optionsMenuPanel.SetActive(false);
        pausePanel.SetActive(false);
    }

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }

    void Update()
    {
        if(!Pause)
            playTime += Time.deltaTime;
    }

    private void LateUpdate()
    {
        int hour = (int)(playTime / 3600);
        int min = (int)(playTime - hour * 3600) / 60;
        int sec = (int)playTime % 60;
        timeText.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", sec);

        if (Input.GetKeyDown(KeyCode.Escape) && gameUI.activeSelf == true)
        {
            if (Pause)
            {
                PauseMenuClose();
            }

            else
            {
                PauseMenuOpen();
            }
        }

        if (player.curHealth >= 0)
        {
            int fullEmptyHeartCount = player.finalMaxHeart / 2;
            int fullHeartCount = player.curHealth / 2;

            int i;

            foreach (Transform child in heartPanel.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            for (i = 0; i < fullEmptyHeartCount; i++)
            {
                Instantiate(fullEmptyHeartPrefab, heartPanel.transform.position + new Vector3(i * 60, 0, 0), Quaternion.identity, heartPanel.transform);
            }


            for (i = 0; i < fullHeartCount; i++)
            {
                Instantiate(fullHeartPrefab, heartPanel.transform.position + new Vector3(i * 60, 0, 0), Quaternion.identity, heartPanel.transform);
            }
            if (player.curHealth % 2 == 1)
            {
                Instantiate(halfHeartPrefab, heartPanel.transform.position + new Vector3(i * 60, 0, 0), Quaternion.identity, heartPanel.transform);
            }

        }

        if (player.hasBlanks >= 0)
        {
            foreach (Transform child in blankBulletPanel.transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < player.hasBlanks; i++)
            {
                Instantiate(blankBulletPrefab, blankBulletPanel.transform.position + new Vector3(i * 50, 0, 0), Quaternion.identity, blankBulletPanel.transform);
            }
        }

        keyText.text = player.key.ToString();
        coinText.text = player.coin.ToString();

        if(player.equipGun != null)
        {
            weaponImage.sprite = player.equipGun.gunSprite;
            weaponImage.color = new Color(weaponImage.color.r, weaponImage.color.g, weaponImage.color.b, 1);

            ammoText.text = player.equipGun.ammo + " / " + player.equipGun.finalMagAmmo;
            curMagText.text = player.equipGun.curMagAmmo.ToString();
        }
        else
        {
            weaponImage.sprite = null;
            weaponImage.color = new Color(weaponImage.color.r, weaponImage.color.g, weaponImage.color.b, 0);
            ammoText.text = "0 / 0";
            curMagText.text = "0";
        }

        if (room != null && room.isBossRoom && boss)
        {
            bossPanel.SetActive(true);
            float bossHealth = (float)boss.curHealth / boss.maxHealth;
            if (bossHealth < 0) bossHealth = 0;
            bossHealthImage.localScale = new Vector3(bossHealth, 1, 1);
        }

        else
        {
            bossPanel.SetActive(false);
        }
    }

    // 아이템 이름, 가격 UI 표시
    public void OnItemPanel(Item item, Vector3 coord)
    {
        itemPanel.transform.position = Camera.main.WorldToScreenPoint(coord);
        itemPanel.SetActive(true);
        itemText.text = item.displayName + ": " +item.price;
    }

    public void OnItemPanel(Gun gun, Vector3 coord)
    {
        itemPanel.transform.position = Camera.main.WorldToScreenPoint(coord);
        itemPanel.SetActive(true);
        itemText.text = gun.displayName + ": " + gun.price;
    }

    public void OffItemPanel()
    {
        itemPanel.SetActive(false);
    }

    public void Gameover()
    {
        Pause = true;
        Cursor.visible = true;

        timeText.text = "시간 : " + timeText.text;
        resultCoinText.text = "자금 : " + coinText.text;
        killCountText.text = "처치 수 : " + killCount.ToString();
        GameoverPanel.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void Play()
    {
        titleMenuPanel.SetActive(false);
        gameUI.SetActive(true);
        Cursor.visible = false;

        Time.timeScale = 1;
        Pause = false;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OptionsMenu()
    {
        optionsMenuPanel.SetActive(true);
    }

    public void TitleMenu()
    {
        titleMenuPanel.SetActive(true);
        pausePanel.SetActive(false);
        optionsMenuPanel.SetActive(false);

        gameUI.SetActive(false);
        Cursor.visible = true;
        Time.timeScale = 0f;
        Pause = true;
    }

    public void PauseMenuOpen()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Pause = true;
    }

    public void PauseMenuClose()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Pause = false;
    }


    public void SetMasterVolume(float value)
    {
        volumeSliders[0].value = value;
        volumeSliders[2].value = value;
        AudioManager.Instance.SetVolume(value, AudioManager.AudioChannel.Master);

    }

    public void SetSfxVolume(float value)
    {
        volumeSliders[1].value = value;
        volumeSliders[3].value = value;
        AudioManager.Instance.SetVolume(value, AudioManager.AudioChannel.Sfx);

    }

    public void Crosshair_Button_Left()
    {
        currentCrosshairNum--;
        

        if(currentCrosshairNum < 0)
        {
            currentCrosshairNum = crosshairs.Length-1;
        }

        Debug.Log(currentCrosshairNum);

        option_CrosshairImage.sprite = crosshairs[currentCrosshairNum];
        pause_CrosshairIamge.sprite = crosshairs[currentCrosshairNum];

        crosshair.GetComponent<SpriteRenderer>().sprite = crosshairs[currentCrosshairNum];

        PlayerPrefs.SetInt("crosshair", currentCrosshairNum);
    }

    public void Crosshair_Button_Right()
    {
        currentCrosshairNum++;
        

        if (currentCrosshairNum >= crosshairs.Length)
        {
            currentCrosshairNum = 0;
        }

        Debug.Log(currentCrosshairNum);

        option_CrosshairImage.sprite = crosshairs[currentCrosshairNum];
        pause_CrosshairIamge.sprite = crosshairs[currentCrosshairNum];

        crosshair.GetComponent<SpriteRenderer>().sprite = crosshairs[currentCrosshairNum];

        PlayerPrefs.SetInt("crosshair", currentCrosshairNum);
    }

}
