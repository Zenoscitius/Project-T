using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState GameState;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ChangeState(GameState.GenerateGrid);
    }

    public void ChangeState(GameState newState)
    {
        GameState = newState;
        switch (newState)
        {
            case GameState.GenerateGrid:
                GridManager.Instance.GenerateGrid();
                break;
            case GameState.SpawnHeroes:
                UnitManager.Instance.SpawnHeroes();
                break;
            case GameState.SpawnEnemies:
                UnitManager.Instance.SpawnEnemies();
                break;
            case GameState.HeroesTurn:
                UnitManager.Instance.RefreshUnits();
                Debug.Log("Start Heroes Turn");
                break;
            case GameState.EnemiesTurn:
                UnitManager.Instance.RefreshUnits();
                Debug.Log("Start Enemies Turn");
                //TODO: Enemy turn logic
                ChangeState(GameState.HeroesTurn);
                break;
            case GameState.Combat:
                UnitManager.Instance.StartCombat();
                break;
            case GameState.Conversation:
                break;
            default:
                break;
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }
}

public enum GameState
{
    GenerateGrid = 0,
    SpawnHeroes = 1,
    SpawnEnemies = 2,
    HeroesTurn = 3,
    EnemiesTurn = 4,
    Combat = 5,
    Conversation = 6
}
