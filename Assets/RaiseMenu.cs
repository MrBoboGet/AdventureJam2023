using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public PokerHandler AssociatedHandler = null;

    public void OnMatch()
    {
        //AssociatedHandler.
        AssociatedHandler.OnMatch();
        gameObject.SetActive(false);
        print("Matching");
    }
    public void OnRaise()
    {
        AssociatedHandler.OnRaise();
        gameObject.SetActive(false);
        print("Raising");
    }
    public void OnFold()
    {
        AssociatedHandler.OnFold();
        gameObject.SetActive(false);
        print("Folding");
    }
}
