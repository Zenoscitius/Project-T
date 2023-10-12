using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    private List<ScriptableUnit> _units, _adjacentUnits;

    public BaseHero SelectedHero;
    public BaseEnemy SelectedEnemy;

    private void Awake()
    {
        Instance = this;

        _units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
    }

    //Spawns a random hero at a pseudorandom location
    public void SpawnHeroes()
    {
        var heroCount = 1;

        for (int i = 0; i < heroCount; i++)
        {
            var randomPrefab = GetRandomUnit<BaseHero>(Faction.Hero);
            var spawnedHero = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.Instance.GetHeroSpawnTile();

            randomSpawnTile.SetUnit(spawnedHero);
        }

        GameManager.Instance.ChangeState(GameState.SpawnEnemies);
    }

    //Spawns a random enemy at a pseudorandom location
    public void SpawnEnemies()
    {
        var enemyCount = 1;

        for (int i = 0; i < enemyCount; i++)
        {
            var randomPrefab = GetRandomUnit<BaseEnemy>(Faction.Enemy);
            var spawnedEnemy = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();

            randomSpawnTile.SetUnit(spawnedEnemy);
        }

        GameManager.Instance.ChangeState(GameState.HeroesTurn);
    }

    //Gets a random unit from a given faction and returns the prefab
    private T GetRandomUnit<T>(Faction faction) where T : BaseUnit
    {
        return (T)_units.Where(u => u.Faction == faction).OrderBy(o => Random.value).First().UnitPrefab;
    }
 
    public void SetSelectedHero(BaseHero hero)
    {
        SelectedHero = hero;
        MenuManager.Instance.ShowSelectedHero(hero);
    }

    public void SetSelectedEnemy(BaseEnemy enemy)
    {
        SelectedEnemy = enemy;
    }

    public void StartCombat(BaseUnit attacker, BaseUnit defender)
    {
        Debug.Log($"Combat started with {attacker.UnitName} and {defender.UnitName}");
        defender.TakeDamage(attacker.attack);
        if(defender.currentHealth <= 0) Destroy(defender.gameObject);
        else if(defender.attackRange >= attacker.attackRange) attacker.TakeDamage(defender.attack);
        UnitManager.Instance.ExhaustSelectedUnit(attacker);
        UnitManager.Instance.CheckEndOfTurn(attacker.Faction);
    }

    public void CheckEndOfTurn(Faction faction)
    {
        int activeUnits = 0;
        foreach (ScriptableUnit unit in _units)
        {
            if (unit.Faction == faction && !unit.IsExhausted())
            {
                Debug.Log("Adding " + unit.UnitPrefab.name + " who " + unit.IsExhausted());
                activeUnits++;
            }
        }
        
        if (activeUnits == 0)
        {
            Debug.Log("No active units, trying to switch turns.");
            if (GameManager.Instance.GameState == GameState.HeroesTurn)
            {
                GameManager.Instance.ChangeState(GameState.EnemiesTurn);
            }
            else
            {
                GameManager.Instance.ChangeState(GameState.HeroesTurn);
            }
            
        }
    }

    public void ExhaustSelectedUnit(BaseUnit selectedUnit)
    {
        foreach (ScriptableUnit unit in _units)
        {
            if (unit.unitName.Equals(selectedUnit.UnitName))
            {
                unit.Exhaust();
            }
        }
        MenuManager.Instance.CloseAllMenus();
    }

    public void RefreshUnits()
    {
        foreach (ScriptableUnit unit in _units)
        {
            unit.Refresh();
        }
    }
  
    public List<BaseUnit> GetUnitsInRange(BaseUnit selectedUnit, Faction targetFaction, Tile selectedTile)
    {
        HashSet<Tile> attackableTiles = GridManager.Instance.GetAttackableTiles(selectedUnit.attackRange, new HashSet<Tile> { selectedTile });
        List<BaseUnit> unitsInRange = new List<BaseUnit>();
        foreach (Tile t in attackableTiles)
        {
            if (t.OccupiedUnit != null)
            {
                if (t.OccupiedUnit.Faction == targetFaction) unitsInRange.Add(t.OccupiedUnit);
            }
        }
        return unitsInRange;
    }
}
