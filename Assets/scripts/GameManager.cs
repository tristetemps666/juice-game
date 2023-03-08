using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public  static GameManager Instance;

    public GameState actual_game_state;

    public float timeScale;

    public enum GameState {
        pause,
        start,
        startLevel,
        game,
        win,
        loose, 
        overview
    }

    void Awake(){
        Instance = this;
    }

    void UpdateGameState(){
        switch(actual_game_state){
            case GameState.pause:
                break;

            case GameState.game:
                break;

            case GameState.start:
                break;

            case GameState.startLevel:
                break;

            case GameState.win:
                break;

            case GameState.loose:
                break;
            
            case GameState.overview:
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = timeScale;
           actual_game_state = GameState.startLevel;
    }
 
    // Update is called once per frame
    void Update()
    {
        
    }
}
