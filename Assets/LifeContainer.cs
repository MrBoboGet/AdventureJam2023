using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeContainer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        bool Player = true;
        foreach(Transform Child in transform)
        {
            foreach(Transform Container in Child)
            {
                if (Player)
                {
                    PlayerHP.Add(Container.gameObject);
                }
                else
                {
                    OpponentHP.Add(Container.gameObject);
                }
            }
            Player = false;
        }
    }
    List<GameObject> PlayerHP = new List<GameObject>();
    List<GameObject> OpponentHP = new List<GameObject>();
    public void DecreasePlayerHP()
    {
        PlayerHP[PlayerHP.Count - 1].SetActive(false);
        PlayerHP.RemoveAt(PlayerHP.Count - 1);
    }
    public void DecreaseOpponentHP()
    {
        OpponentHP[OpponentHP.Count - 1].SetActive(false);
        OpponentHP.RemoveAt(OpponentHP.Count - 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
