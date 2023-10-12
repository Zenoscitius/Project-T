using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatScene : MonoBehaviour
{
    public TextMeshProUGUI attackerName, attackerHit, attackerCrit, attackerDamage, attackerHealthNumber, attackerWeaponName, defenderName, defenderHit, defenderCrit, defenderDamage, defenderHealthNumber, defenderWeaponName;
    public Image landSprite;
    public CombatSprite attackerSprite, defenderSprite;
    public HealthBar attackerHealthBar, defenderHealthBar;

    public void InitializeCombatScene(BaseUnit attacker, BaseUnit defender, Tile combatTile)
    {
        attackerName.text = attacker.UnitName;
        attackerHit.text = "HIT: " + attacker.hit.ToString();
        attackerCrit.text = "CRIT: " + attacker.crit.ToString();
        attackerDamage.text = "DMG: " + attacker.might.ToString();
        attackerHealthNumber.text = attacker.currentHealth.ToString();
        attackerWeaponName.text = attacker.activeWeapon.ItemName;
        attackerSprite.unitSprite.sprite = attacker.GetComponentInChildren<SpriteRenderer>().sprite;
        attackerSprite.unitAnimator.runtimeAnimatorController = attacker.GetComponent<Animator>().runtimeAnimatorController;
        attackerHealthBar.SetMaxHealth(attacker.maxHealth);
        attackerHealthBar.SetHealth(attacker.currentHealth);

        defenderName.text = defender.UnitName;
        defenderHit.text = "HIT: " + defender.hit.ToString();
        defenderCrit.text = "CRIT: " + defender.crit.ToString();
        defenderDamage.text = "DMG: " + defender.might.ToString();
        defenderHealthNumber.text = defender.currentHealth.ToString();
        defenderWeaponName.text = defender.activeWeapon.ItemName;
        defenderSprite.unitSprite.sprite = defender.GetComponentInChildren<SpriteRenderer>().sprite;
        defenderSprite.unitAnimator.runtimeAnimatorController = defender.GetComponent<Animator>().runtimeAnimatorController;
        defenderHealthBar.SetMaxHealth(defender.maxHealth);
        defenderHealthBar.SetHealth(attacker.currentHealth);
    }
    public void PlayCombatAnimation(BaseUnit attacker, BaseUnit defender)
    {

        /*
        int toHit = Random.Range(0, 100);
        if (toHit + attacker.hit > defender.avo)
        {
            int toCrit = Random.Range(0, 100);
            if (toCrit <= attacker.crit)
            {

            }
            else
            {

            }
        }
        */
        Debug.Log($"Combat started with {attacker.UnitName} and {defender.UnitName}");
        attackerSprite.unitAnimator.SetTrigger("attack");
        Debug.Log("Attack animation play");
        StartCoroutine(WaitCoroutine());
        defenderSprite.unitAnimator.SetTrigger("damaged");
        Debug.Log("Defend animation play");
        StartCoroutine(WaitCoroutine());
        defender.TakeDamage(attacker.attack);
        if (defender.currentHealth <= 0)
        {
            defender.currentHealth = 0;
            Destroy(defender.gameObject);
        }
        else if (defender.attackRange >= attacker.attackRange)
        {
            defenderSprite.unitAnimator.SetTrigger("attack");
            StartCoroutine(WaitCoroutine());
            defenderSprite.unitAnimator.SetTrigger("damaged");
            StartCoroutine(WaitCoroutine());
            attacker.TakeDamage(defender.attack);
        }

    }

    IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(200);
    }
}
