using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleSystem : MonoBehaviour
{
    public TextMeshProUGUI battleMessage;
    public Slider playerHealthBar;
    public Slider enemyHealthBar;
    public Button attackButton;
    public Button defendButton;
    public Button itemButton;

    public Animator soldierAnimator;  
    public Animator enemyAnimator;    

    private int playerMaxHealth = 100;
    private int enemyMaxHealth = 100;
    private int playerCurrentHealth;
    private int enemyCurrentHealth;

    void Start()
    {
        
        playerCurrentHealth = playerMaxHealth;
        enemyCurrentHealth = enemyMaxHealth;

        
        playerHealthBar.maxValue = playerMaxHealth;
        playerHealthBar.value = playerCurrentHealth;

        enemyHealthBar.maxValue = enemyMaxHealth;
        enemyHealthBar.value = enemyCurrentHealth;

        
        attackButton.onClick.AddListener(OnAttackButton);
        defendButton.onClick.AddListener(OnDefendButton);
        itemButton.onClick.AddListener(OnItemButton);

        battleMessage.text = "A wild enemy appears!";
    }

    void OnAttackButton()
    {
        
        soldierAnimator.SetBool("isAttacking", true);

        int damage = Random.Range(10, 20);
        enemyCurrentHealth -= damage;
        enemyHealthBar.value = enemyCurrentHealth;

        battleMessage.text = "You attacked the enemy for " + damage + " damage!";

        
        Invoke("ResetPlayerAttack", 0.5f);  

        
        EnemyReceiveDamage();

        if (enemyCurrentHealth <= 0)
        {
            StartCoroutine(HandleVictory());
        }
        else
        {
            
            Invoke("EnemyTurn", 1f);
        }
    }

    void ResetPlayerAttack()
    {
        soldierAnimator.SetBool("isAttacking", false);
    }

    void OnDefendButton()
    {
        
        battleMessage.text = "You defend against the enemy's next attack!";

        
        Invoke("EnemyTurn", 1f);
    }

    void OnItemButton()
    {
        
        int heal = Random.Range(10, 20);
        playerCurrentHealth += heal;
        if (playerCurrentHealth > playerMaxHealth)
            playerCurrentHealth = playerMaxHealth;

        playerHealthBar.value = playerCurrentHealth;

        battleMessage.text = "You used an item and healed for " + heal + " health!";

        
        Invoke("EnemyTurn", 1f);
    }

    void EnemyTurn()
    {
        int enemyAction = Random.Range(0, 2); 

        if (enemyAction == 0)
        {
            EnemyAttack();
        }
        else if (enemyAction == 1)
        {
            EnemyHeal();
        }

        if (playerCurrentHealth <= 0)
        {
            StartCoroutine(HandleDefeat());
        }
    }

    void EnemyAttack()
    {
        
        enemyAnimator.SetBool("isAttacking", true);

        int damage = Random.Range(10, 20);
        playerCurrentHealth -= damage;
        playerHealthBar.value = playerCurrentHealth;

        battleMessage.text = "The enemy attacks you for " + damage + " damage!";

        
        Invoke("ResetEnemyAttack", 0.5f);  

        
        OnReceiveDamage();

        if (playerCurrentHealth <= 0)
        {
            StartCoroutine(HandleDefeat());
        }
    }

    void ResetEnemyAttack()
    {
        enemyAnimator.SetBool("isAttacking", false);
    }

    void EnemyHeal()
    {
        int heal = Random.Range(5, 15);
        enemyCurrentHealth += heal;
        if (enemyCurrentHealth > enemyMaxHealth)
            enemyCurrentHealth = enemyMaxHealth;

        enemyHealthBar.value = enemyCurrentHealth;

        battleMessage.text = "The enemy heals itself for " + heal + " health!";
    }

    void OnReceiveDamage()
    {
        
        soldierAnimator.SetBool("isDamaged", true);

        
        Invoke("ResetDamageAnimation", 0.5f);  
    }

    void ResetDamageAnimation()
    {
        soldierAnimator.SetBool("isDamaged", false);
    }

    void EnemyReceiveDamage()
    {
        
        enemyAnimator.SetBool("isDamaged", true);

        
        Invoke("ResetEnemyDamageAnimation", 0.5f);  
    }

    void ResetEnemyDamageAnimation()
    {
        enemyAnimator.SetBool("isDamaged", false);
    }

    IEnumerator HandleVictory()
    {
        
        enemyAnimator.SetBool("isDead", true);

        battleMessage.text = "You have defeated the enemy!";
        DisableBattleUI();

        yield return new WaitForSeconds(2f); 

        
        SceneManager.LoadScene("VictoryScene"); 
    }

    IEnumerator HandleDefeat()
    {
        
        soldierAnimator.SetBool("isDead", true);

        yield return new WaitForSeconds(1f);  

        battleMessage.text = "You have been defeated!";
        DisableBattleUI();

        yield return new WaitForSeconds(2f);  

        
        SceneManager.LoadScene("DefeatScene"); 
    }

    void DisableBattleUI()
    {
        
        attackButton.interactable = false;
        defendButton.interactable = false;
        itemButton.interactable = false;
    }
}
