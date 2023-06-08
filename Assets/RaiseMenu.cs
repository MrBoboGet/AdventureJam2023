using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AssociatedHandler = FindObjectOfType<PokerHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if(AssociatedHandler == null)
        {
            AssociatedHandler = FindObjectOfType<PokerHandler>();
        }
    }

    public PokerHandler AssociatedHandler = null;

    public void SetPot(int CurrentStake,int PlayerStake)
    {
        GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Current stake: " + CurrentStake + "$\nYour stake: "+PlayerStake+"$";
    }

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
