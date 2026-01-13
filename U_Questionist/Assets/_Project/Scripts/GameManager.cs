using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private MenuState menuState;
    private IState currentState;
    private IState previousState;

    private void Start()
    {
        currentState = menuState;
        currentState.Enter();
    }

    private void Update()
    {
        if(currentState!=null)
            currentState.Update();
        
    }

    public void ChangeState(IState newState)
    {
        previousState = currentState;
        
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
    

    public IState GetCurrentState()
    {
        return currentState;
    }
    
    
    #region Singleton

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;
    }
    

    #endregion

}
 

public interface IState
{
    void Enter();
    void Update();
    void Exit();
}
