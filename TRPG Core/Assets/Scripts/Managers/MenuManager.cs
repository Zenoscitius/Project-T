using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] private GameObject _selectedHeroObject, _tileObject, _tileUnitObject;
    [SerializeField] private GameObject _preperationsMenu, _generalMenu, _combatMenu, _conversationScene;
    [SerializeField] private ActionMenu _actionMenu;

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
        HashSet<Tile> attackableTiles = GridManager.Instance.GetAttackableTiles(UnitManager.Instance.SelectedHero.attackRange,new HashSet<Tile> {activeTile});
        HashSet<BaseEnemy> attackableUnits = new HashSet<BaseEnemy>();
        foreach(Tile t in attackableTiles)
        {
            if(t.OccupiedUnit != null)
            {
                if (t.OccupiedUnit.Faction == Faction.Enemy) attackableUnits.Add((BaseEnemy)t.OccupiedUnit);
            }
        }
        _actionMenu.attackButton.interactable = (attackableUnits.Count != 0);
       
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
        _generalMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        //EventSystem.current.SetSelectedGameObject (_generalFirstButton);
    }
    
    //TODO combat menu: select viable targets, preview related combat stats (HP,Mt,Hit,Crit, your weapon, monster's weapon)
    public void ShowCombatMenu()
    {
        if (UnitManager.Instance.SelectedHero == null) throw new ArgumentException("Selected Hero cannot be null");
        _combatMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        //EventSystem.current.SetSelectedGameObject(_combatFirstButton);
    }

    //TODO displays the conversation with the given conversation number **determine best way through tutorial
    public void StartConversation(int conversationNumber)
    {
        if (conversationNumber == 0) throw new ArgumentException("Parameter cannot be null");
        _conversationScene.SetActive(true);
    }

    public void Wait()
    {
        activeTile.SetUnit(UnitManager.Instance.SelectedHero);
        //TODO: exhaust the hero, check for auto turn end
        GridManager.Instance.ClearMovementOverlay();
        UnitManager.Instance.SetSelectedHero(null);
        _actionMenu.gameObject.SetActive(false);
    }

    public void Attack()
    {
        //TODO: transfer attack script here with no automove
    }
}
