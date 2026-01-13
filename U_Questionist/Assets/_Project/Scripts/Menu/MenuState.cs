using UnityEditor;
using UnityEngine;

public class MenuState : MonoBehaviour, IState
{
    public Transform MenuScreen;
    
    public void Enter()
    { 
        MenuScreen.gameObject.SetActive(true);
    }

    void IState.Update()
    {
        
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }
}
