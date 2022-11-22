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

    public void StartCombat()
    {
        Debug.Log($"Combat started with {SelectedHero.UnitName} and {SelectedEnemy.UnitName}");
        SelectedEnemy.TakeDamage(SelectedHero.attack);
        if(SelectedEnemy.currentHealth <= 0) Destroy(SelectedEnemy.gameObject);
        else if(SelectedEnemy.attackRange >= SelectedHero.attackRange) SelectedHero.TakeDamage(SelectedEnemy.attack);
        GameManager.Instance.ChangeState(GameState.HeroesTurn);
    }

    /**
    public List<T> GetAdjacentUnits<T>(Faction faction) where T : BaseUnit
    {
        HashSet<Tile> attackableTiles = GridManager.Instance.GetAttackableTiles(UnitManager.Instance.SelectedHero.attackRange, new HashSet<Tile> { UnitManager.Instance.SelectedHero.OccupiedTile });
        HashSet<BaseEnemy> attackableUnits = new HashSet<BaseEnemy>();
        foreach (Tile t in attackableTiles)
        {
            if (t.OccupiedUnit != null)
            {
                if (t.OccupiedUnit.Faction == Faction.Enemy) attackableUnits.Add((BaseEnemy)t.OccupiedUnit);
            }
        }
    }
    **/
}
