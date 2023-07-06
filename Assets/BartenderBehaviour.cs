using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BartenderBehaviour : OpponentScript
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override HandAnimation GetHandAnimation(PokerState CurrentPokerState,List<GameObject> Cards, OpponentScript Opponent)
    {
        return (new HandAnimation(Cards, Opponent));
    }
}
