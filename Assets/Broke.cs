using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Broke : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBroke()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(GlobalTransitionInfo.BattleScene);
    }
}
