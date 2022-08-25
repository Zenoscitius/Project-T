using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public string TileName;
    public int tileX;
    public int tileY;
    public int moveCost;
    public GameObject _highlight;
    public GameObject _movableHighlight;
    public GameObject _attackableHighlight;
    [SerializeField] private bool _isWalkable;

    //Linked Tile Map Variables
    public Tile tileLeft = null;
    public Tile tileRight = null;
    public Tile tileTop = null;
    public Tile tileBottom = null;
    public HashSet<Tile> adjacentTiles = new HashSet<Tile>();

    public BaseUnit OccupiedUnit;
    public bool Walkable => _isWalkable && OccupiedUnit == null;

    private void OnMouseEnter()
    {
        _highlight.SetActive(true);
        MenuManager.Instance.ShowTileInfo(this);
    }

    private void OnMouseExit()
    {
        _highlight.SetActive(false);
        MenuManager.Instance.ShowTileInfo(null);
    }

    void OnMouseDown()
    {
        //Only accept selection on the heroes turn
        if (GameManager.Instance.GameState != GameState.HeroesTurn) return;

        //If a tile has a unit on it when clicked and...
        if (OccupiedUnit != null)
        {
            //Logic if hero is selected on the map
            if(OccupiedUnit.Faction == Faction.Hero)
            {
                
                UnitManager.Instance.SetSelectedHero((BaseHero)OccupiedUnit);
                GridManager.Instance.ShowMovableTiles(OccupiedUnit.OccupiedTile, OccupiedUnit.speed, OccupiedUnit.attackRange);                
            }
            else
            {
                //Logic if a highlighted enemy is selected after a hero has been selected
                if (UnitManager.Instance.SelectedHero != null)
                {
                    if (_movableHighlight.activeSelf || _attackableHighlight.activeSelf)
                    {
                        UnitManager.Instance.SetSelectedEnemy((BaseEnemy)OccupiedUnit);
                        GridManager.Instance.MoveInRange();
                        GameManager.Instance.ChangeState(GameState.Combat);
                    }
                    UnitManager.Instance.SetSelectedHero(null);
                    UnitManager.Instance.SetSelectedEnemy(null);
                    GridManager.Instance.ClearMovementOverlay();
                }
            }
        }
        else
        {
            //A highlighted tile with no unit on it is selected after a hero is selected
            if (UnitManager.Instance.SelectedHero != null)
            {
                if (_movableHighlight.activeSelf)
                {
                    MenuManager.Instance.ShowActionMenu(this);
                }
                
            }
        }
    }
    public void SetUnit(BaseUnit unit)
    {
        if (unit.OccupiedTile != null) unit.OccupiedTile.OccupiedUnit = null;
        unit.transform.position = transform.position;
        OccupiedUnit = unit;
        unit.OccupiedTile = this;
    }
    public void ShowMovable()
    {
        _movableHighlight.SetActive(true);
    }
    public void HideHighlights()
    {
        _movableHighlight.SetActive(false);
        _attackableHighlight.SetActive(false);
    }
    public void ShowAttackable()
    {
        _attackableHighlight.SetActive(true);
    }
}
