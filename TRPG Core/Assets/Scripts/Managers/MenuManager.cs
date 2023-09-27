using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using JetBrains.Annotations;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] private GameObject _selectedHeroObject, _tileObject, _tileUnitObject;
    [SerializeField] private GameObject _preperationsMenu, _generalMenu, _conversationScene;
    [SerializeField] private ActionMenu _actionMenu;
    [SerializeField] private InventoryMenu _inventoryMenu;
    [SerializeField] private ItemMenu _itemMenu;
    [SerializeField] private AttackMenu _attackMenu;
    private List<BaseUnit> _targets;
    private int _targetIndex = -1;
    private ScriptableItem _activeItem;

    private Tile activeTile;
    private void Awake()
    {
        Instance = this;
    }

    public void ShowSelectedHero(BaseHero hero)
    {
        if(hero == null)
        {
            _selectedHeroObject.SetActive(false);
            return;
        }
        _selectedHeroObject.GetComponentInChildren<Text>().text = hero.UnitName;
        _selectedHeroObject.SetActive(true);
    }

    public void ShowTileInfo(Tile tile)
    {
        if (tile == null)
        {
            _tileObject.SetActive(false);
            _tileUnitObject.SetActive(false);
            return;
        }
        _tileObject.GetComponentInChildren<Text>().text = tile.TileName;
        _tileObject.SetActive(true);

        if (tile.OccupiedUnit)
        {
            _tileUnitObject.GetComponentInChildren<Text>().text = tile.OccupiedUnit.UnitName;
            _tileUnitObject.SetActive(true);
        }
    }

    // TODO: preperations menu: pick units, items, support, check map, save
    public void ShowPreperationsMenu()
    {

    }

    //TODO: Build contextual menu for character actions: attack, talk, item, wait, protect(?)
    public void ShowActionMenu(Tile tile)
    {
        activeTile = tile;
        if (UnitManager.Instance.SelectedHero == null) throw new ArgumentException("Selected Hero cannot be null");

        //Determine if attack is active and populate attack list
        _targets = UnitManager.Instance.GetUnitsInRange(UnitManager.Instance.SelectedHero, Faction.Enemy, tile);
        _actionMenu.attackButton.interactable = (_targets.Count != 0);

        //Determine if talk is active and populate talk list
        HashSet<BaseHero> talkList = new HashSet<BaseHero>();
        foreach (Tile t in activeTile.adjacentTiles)
        {
            if (t.OccupiedUnit != null)
            {
                //TODO: look if the targeted hero has available dialogue with the selected hero
                //BUG: selected hero can currently talk to himself
                if (t.OccupiedUnit.Faction == Faction.Hero) talkList.Add((BaseHero)t.OccupiedUnit);
            }
            
        }
        _actionMenu.talkButton.interactable = (talkList.Count != 0);

        _actionMenu.gameObject.SetActive(true);
    }

    //TODO empty area: unit, status, guide, options, suspend, end
    public void ShowGeneralMenu()
    {
        Debug.Log("This is where the general menu would go... IF I HAD ONE");
        //_generalMenu.SetActive(true);
        //EventSystem.current.SetSelectedGameObject(null);
        //EventSystem.current.SetSelectedGameObject (_generalFirstButton);
    }
    
    //TODO combat menu: select viable targets, preview related combat stats (HP,Mt,Hit,Crit, your weapon, monster's weapon)
    public void ShowCombatMenu()
    {
        if (UnitManager.Instance.SelectedHero == null) throw new ArgumentException("Selected Hero cannot be null");
        if (_targets.Count == 0) throw new ArgumentException("How did you get here with no targets?");

        //pull up the attack menu and select the first target in the list
        if (_targetIndex == -1)
        {
            _targetIndex = 0;
        }
        BaseUnit hero = UnitManager.Instance.SelectedHero;
        BaseUnit enemy = _targets[_targetIndex];
        _attackMenu.heroName.text = hero.name;
        _attackMenu.heroHealth.text = hero.currentHealth.ToString() + "/" + hero.maxHealth.ToString();
        _attackMenu.heroWeaponName.text = hero.activeWeapon.ItemName;
        _attackMenu.heroWeaponDur.text = hero.activeWeapon.currentDurability.ToString() + "/" + hero.activeWeapon.maxDurability.ToString();
        _attackMenu.heroMt.text = "MT: " + hero.might.ToString();
        _attackMenu.heroHit.text = "HIT: " + hero.hit.ToString();
        _attackMenu.heroCrit.text = "CRIT: " + hero.crit.ToString();
        _attackMenu.heroWeaponSprite.sprite = hero.activeWeapon.MenuSprite;
        _attackMenu.enemyName.text = enemy.name;
        _attackMenu.enemyHealth.text = enemy.currentHealth.ToString() + "/" + enemy.maxHealth.ToString();
        _attackMenu.enemyWeaponName.text = enemy.activeWeapon.ItemName;
        _attackMenu.enemyWeaponDur.text = enemy.activeWeapon.currentDurability.ToString() + "/" + enemy.activeWeapon.maxDurability.ToString();
        _attackMenu.enemyMt.text = "MT: " + enemy.might.ToString();
        _attackMenu.enemyHit.text = "HIT: " + enemy.hit.ToString();
        _attackMenu.enemyCrit.text = "CRIT: " + enemy.crit.ToString();
        _attackMenu.enemyWeaponSprite.sprite = enemy.activeWeapon.MenuSprite;
        _attackMenu.gameObject.SetActive(true);
        _attackMenu.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        //EventSystem.current.SetSelectedGameObject(_combatFirstButton);
    }

    public void NextTarget()
    {
        if (_targets.Count == _targetIndex+1)
        {
            _targetIndex = 0;
        }
        else
        {
            _targetIndex++;
        }
        ShowCombatMenu();
    }
    public void PrevTarget() 
    {
        if ( _targetIndex-1 < 0)
        {
            _targetIndex = _targets.Count - 1;
        }
        else
        {
            _targetIndex--;
        }
        ShowCombatMenu();
    }
    public void Attack()
    {
        CloseAllMenus();
        activeTile.SetUnit(UnitManager.Instance.SelectedHero);
        GridManager.Instance.ClearMovementOverlay();
        UnitManager.Instance.StartCombat(UnitManager.Instance.SelectedHero, _targets[_targetIndex]);
    }

    //TODO displays the conversation with the given conversation number **determine best way through tutorial
    public void StartConversation(int conversationNumber)
    {
        if (conversationNumber == 0) throw new ArgumentException("Parameter cannot be null");
        _conversationScene.SetActive(true);
    }

    public void Talk()
    {
        //TODO: initialize conversation with selected ally
    }

    //Item sub-menu (use/equip, trade>target selector, discard > confirm)
    public void ShowInventory()
    {
        Inventory activeInventory = UnitManager.Instance.SelectedHero.itemInventory;
        
        for (int i =0; i<5; i++)
        {
            var itemSlot = _inventoryMenu.MenuItemSlotArray[i];
            if (activeInventory.itemArray[i] != null)
            {
               
               itemSlot.itemName.text = activeInventory.itemArray[i].ItemName;
               itemSlot.itemDurrability.text = $"{activeInventory.itemArray[i].currentDurability} / {activeInventory.itemArray[i].maxDurability}";
               itemSlot.itemSprite.sprite = activeInventory.itemArray[i].MenuSprite;
               itemSlot.selectorButton.interactable = true;
            }
            else
            {
               itemSlot.itemName.text = null;
               itemSlot.itemDurrability.text = null;
               itemSlot.itemSprite.sprite = null;
               itemSlot.selectorButton.interactable = false;
            }
        }
        _inventoryMenu.MenuItemSlotArray[0].selectorButton.Select();
        _inventoryMenu.gameObject.SetActive(true);
    }

    public void SelectItem(int itemNumber)
    {
        //set active item
        _activeItem = UnitManager.Instance.SelectedHero.itemInventory.itemArray[itemNumber];
        if (_activeItem == null) return;

        //display context menu adjacent to item, button pos + offset x/y

        Vector3 invMenuPosition = _inventoryMenu.MenuItemSlotArray[itemNumber].itemName.transform.position;
        _itemMenu.transform.position = invMenuPosition;
        _itemMenu.gameObject.SetActive(true);
    }

    public void UseItem()
    {
        ScriptableConsummable _activeConsummable = (ScriptableConsummable)_activeItem;
        _activeConsummable.UseItem(UnitManager.Instance.SelectedHero);
        UnitManager.Instance.ExhaustSelectedUnit(UnitManager.Instance.SelectedHero);
        CloseAllMenus();
    }

    public void EquipItem()
    {
        UnitManager.Instance.SelectedHero.SwitchWeapon((ScriptableWeapon)_activeItem);
        CloseAllMenus();
    }

    public void TradeItem()
    {
        CloseAllMenus();
    }

    public void DiscardItem()
    {
        CloseAllMenus();
    }

    public void Wait()
    {
        activeTile.SetUnit(UnitManager.Instance.SelectedHero);
        UnitManager.Instance.ExhaustSelectedUnit(UnitManager.Instance.SelectedHero);
        GridManager.Instance.ClearMovementOverlay();
        UnitManager.Instance.CheckEndOfTurn(Faction.Hero);
        UnitManager.Instance.SetSelectedHero(null);
        CloseAllMenus();
    }

    public void CloseAllMenus()
    {
            //_preperationsMenu.SetActive(false);
            //_generalMenu.SetActive(false);
            _attackMenu.gameObject.SetActive(false);
            _actionMenu.gameObject.SetActive(false);
            _itemMenu.gameObject.SetActive(false);
            _inventoryMenu.gameObject.SetActive(false);
    }
    
}
