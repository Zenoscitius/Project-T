using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttackMenu : MonoBehaviour
{
    public TextMeshProUGUI heroName, heroHealth, heroWeaponName, heroWeaponDur, heroMt, heroHit, heroCrit;
    public TextMeshProUGUI enemyName, enemyHealth, enemyWeaponName, enemyWeaponDur, enemyMt, enemyHit, enemyCrit;
    public Image heroWeaponSprite, enemyWeaponSprite;
    public Button prevButton, nextButton, attackButton;

    public void UpdateHeroInfo(BaseHero hero)
    {
        heroName.text = hero.name;
        heroHealth.text = hero.currentHealth.ToString();
        heroWeaponName.text = hero.activeWeapon.ItemName;
        heroWeaponDur.text = hero.activeWeapon.currentDurability.ToString();
        heroMt.text = hero.might.ToString();
        heroHit.text = hero.hit.ToString();
        heroCrit.text = hero.crit.ToString();
        heroWeaponSprite.sprite = hero.activeWeapon.MenuSprite;
    }

    public void UpdateEnemyInfo(BaseEnemy enemy)
    {
        enemyName.text = enemy.name;
        enemyHealth.text = enemy.currentHealth.ToString();
        enemyWeaponName.text = enemy.activeWeapon.ItemName;
        enemyWeaponDur.text = enemy.activeWeapon.currentDurability.ToString();
        enemyMt.text = enemy.might.ToString();
        enemyHit.text = enemy.hit.ToString();
        enemyCrit.text = enemy.crit.ToString();
        enemyWeaponSprite.sprite = enemy.activeWeapon.MenuSprite;
    }
}