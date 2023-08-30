using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/*Manages and updates the HUD, which contains your health bar, coins, etc*/

public class HUD : MonoBehaviour
{
    [Header ("Reference")]
    public Animator animator;
    public TextMeshProUGUI coinsMesh;
    public TextMeshProUGUI levelMesh;
    public TextMeshProUGUI attributePointsMesh;
    public TextMeshProUGUI maxHealthAndManaMesh;
    public TextMeshProUGUI maxExpMesh;
    public TextMeshProUGUI externalStatsMesh;
    public TextMeshProUGUI parryTimerMesh;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject manaBar;
    [SerializeField] private GameObject xpBar;
    [SerializeField] private Image inventoryItemGraphic;
    [SerializeField] private GameObject startUp;

    private float manaBarWidth;
    private float manaBarWidthEased; //Easing variables slowly ease towards a number
    private float xpBarWidth;
    private float xpBarWidthEased; //Easing variables slowly ease towards a number
    [System.NonSerialized] public Sprite blankUI; //The sprite that is shown in the UI when you don't have any items
    private float coins;
    private float coinsEased;
    private float level;
    private float levelEased;
    private float attributePoints;
    private float attributePointsEased;
    private float healthBarWidth;
    private float healthBarWidthEased;
    [System.NonSerialized] public string loadSceneName;
    [System.NonSerialized] public bool resetPlayer;

    void Start()
    {
        //Set all bar widths to 1, and also the smooth variables.
        healthBarWidth = 1;
        healthBarWidthEased = healthBarWidth;
        manaBarWidth = 1;
        manaBarWidthEased = manaBarWidth;
        xpBarWidth = 1;
        xpBarWidthEased = xpBarWidth;
        coins = (float)NewPlayer.Instance.coins;
        coinsEased = coins;
        level = (float)NewPlayer.Instance.GetLevel();
        levelEased = level;
        attributePoints = (float)NewPlayer.Instance.attributePoints;
        attributePointsEased = attributePoints;
        blankUI = inventoryItemGraphic.GetComponent<Image>().sprite;
    }

    void Update()
    {
        //Update coins text mesh to reflect how many coins the player has! However, we want them to count up.
        coinsMesh.text = Mathf.Round(coinsEased).ToString();
        levelMesh.text = Mathf.Round(levelEased).ToString();
        NewPlayer player = NewPlayer.Instance;
        maxHealthAndManaMesh.text = (int) player.health + "/" + (int) player.externalStats[0]
            + "\n" + (int) player.mana + "/" + (int) player.externalStats[2];
        maxExpMesh.text = player.GetXp() + "/" + 10*(int)Mathf.Pow(player.GetLevel() + 1, 2);
        externalStatsMesh.text = "hp = " + ((int)(10*player.externalStats[0]))/10f;
        for (int i = 1; i < player.externalStats.Length; i++)
        {
            externalStatsMesh.text += "\n";

            if (i == 1)
                externalStatsMesh.text += "de = ";
            else if (i == 2)
                externalStatsMesh.text += "ma = ";
            else if (i == 3)
                externalStatsMesh.text += "ms = ";
            else if (i == 4)
                externalStatsMesh.text += "ar = ";
            else if (i == 5)
                externalStatsMesh.text += "pd = ";
            else if (i == 6)
                externalStatsMesh.text += "md = ";
            else if (i == 7)
                externalStatsMesh.text += "fe = ";
            else if (i == 8)
                externalStatsMesh.text += "cr = ";
            else if (i == 9)
                externalStatsMesh.text += "cd = ";
            else if (i == 10)
                externalStatsMesh.text += "jp = ";

            externalStatsMesh.text += "" + ((int)(10*player.externalStats[i]))/10f;
            if (i == 3 || i == 4 || i == 8)
                externalStatsMesh.text += "%";
            else if (i == 5 || i == 6)
                externalStatsMesh.text += "dmg";
        }

        parryTimerMesh.text = "tl = " + ((int)(10*player.parryTimer[0]))/10f;
        for (int i = 1; i < player.parryTimer.Length; i++)
        {
            parryTimerMesh.text += "\n";

            if (i == 1)
                parryTimerMesh.text += "tr = ";
            else if (i == 2)
                parryTimerMesh.text += "bl = ";
            else if (i == 3)
                parryTimerMesh.text += "br = ";

            parryTimerMesh.text += "" + ((int)(100*player.parryTimer[i]))/100f;
        }

        attributePointsMesh.text = Mathf.Round(attributePointsEased).ToString();
        coinsEased += ((float)NewPlayer.Instance.coins - coinsEased) * Time.deltaTime * 5f;
        levelEased += ((float)NewPlayer.Instance.GetLevel() - levelEased) * Time.deltaTime * 5f;
        attributePointsEased += ((float)NewPlayer.Instance.attributePoints - attributePointsEased) * Time.deltaTime * 5f;

        if (coinsEased >= coins)
        {
            animator.SetTrigger("getGem");
            coins = coinsEased + 1;
        }
        if (levelEased >= level)
        {
            animator.SetTrigger("getGem");
            level = levelEased + 1;
        }
        if (attributePointsEased >= attributePoints)
        {
            animator.SetTrigger("getGem");
            attributePoints = attributePointsEased + 1;
        }

        //Controls the width of the health bar based on the player's total health
        healthBarWidth = (float)NewPlayer.Instance.health / (float)NewPlayer.Instance.externalStats[0];
        healthBarWidthEased += (healthBarWidth - healthBarWidthEased) * 10f * Time.deltaTime * healthBarWidthEased;
        if (healthBarWidthEased < 0)
            healthBarWidthEased = 0;
        healthBar.transform.localScale = new Vector2(healthBarWidthEased, 1);

        //Controls the width of the mana bar based on the player's total mana
        manaBarWidth = (float)NewPlayer.Instance.mana / (float)NewPlayer.Instance.externalStats[2];
        manaBarWidthEased += (manaBarWidth - manaBarWidthEased) * 10f * Time.deltaTime * manaBarWidthEased;
        manaBar.transform.localScale = new Vector2(manaBarWidthEased, 1);
        
        if (NewPlayer.Instance.GetXp() == 0)
        {
            xpBarWidth = 0f;
            xpBarWidthEased = 0f;
        }
        else if (NewPlayer.Instance.GetXp() <= 10*System.Math.Pow((NewPlayer.Instance.GetLevel()+1),2))
        {
            xpBarWidth = (float) (NewPlayer.Instance.GetXp() / (10*System.Math.Pow((NewPlayer.Instance.GetLevel()+1),2)));
            xpBarWidthEased += (xpBarWidth - xpBarWidthEased) * 5f * Time.deltaTime;
        }
        else
        {
            xpBarWidth = 1f;
            xpBarWidthEased += (xpBarWidth - xpBarWidthEased) * 5f * Time.deltaTime;
        }
        xpBar.transform.localScale = new Vector2(xpBarWidthEased, 1);
    }

    public void HealthBarHurt()
    {
        animator.SetTrigger("hurt");
    }

    public void SetInventoryImage(Sprite image)
    {
        inventoryItemGraphic.sprite = image;
    }

    void ResetScene()
    {
        if (GameManager.Instance.CheckForInventoryItem("reachedCheckpoint") != null)
        {
            //Send player back to the checkpoint if they reached one!
            NewPlayer.Instance.ResetLevel();
        }
        else
        {
            //Reload entire scene
            SceneManager.LoadScene(loadSceneName);
        }
    }
}
