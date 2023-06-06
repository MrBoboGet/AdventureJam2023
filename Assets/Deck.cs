using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Text.RegularExpressions;

public enum CardType
{
    Spade = 0,
    Diamond = 1,
    Club = 2,
    Heart = 3,
    Null
}

public class Card
{
    public int Value = -1;
    public CardType Type = CardType.Null;
}

public class Deck : MonoBehaviour
{
    public List<Sprite> DeckSprites;
    public Sprite CardBack;

    List<Card> m_CurrentDeck;

    List<Dictionary<CardType, int>> m_SpriteMap = new List<Dictionary<CardType, int>>();
    // Start is called before the first frame update
    void Awake()
    {
        for(int i = 0; i < 13;i++)
        {
            m_SpriteMap.Add(new Dictionary<CardType, int>());
        }
        int Index = 0;
        foreach(Sprite Sprite in DeckSprites)
        {
            CardType Type = CardType.Heart;
            if(Sprite.name.Contains("c"))
            {
                Type = CardType.Club;
            }
            else if(Sprite.name.Contains("d"))
            {
                Type = CardType.Diamond;
            }
            else if(Sprite.name.Contains("s"))
            {
                Type = CardType.Spade;
            }
            Regex StringRegex = new Regex(@"\d\d\d");
            string NumberString = StringRegex.Match(Sprite.name).Groups[0].Value;
            int IntNumber = int.Parse(NumberString);
            m_SpriteMap[IntNumber][Type] = Index;
            Index += 1;
        }


        ResetDeck();
    }

    //shamelessly stolen from https://stackoverflow.com/questions/273313/randomize-a-listt
    private static System.Random rng = new System.Random();
    static void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    List<Card> p_CreateDeck()
    {
        List<Card> ReturnValue = new List<Card>();
        for(int i = 1; i <= 13;i++)
        {
            foreach(CardType Type in new CardType[]{ CardType.Club,CardType.Diamond,CardType.Heart,CardType.Spade})
            {
                Card NewCard = new Card();
                NewCard.Type = Type;
                NewCard.Value = i;
                ReturnValue.Add(NewCard);
            }
        }
        Shuffle(ReturnValue);
        return (ReturnValue);
    }
    public void ResetDeck()
    {
        m_CurrentDeck = p_CreateDeck();
    }
    public Sprite GetSprite(Card AssociatedCard)
    {
        return (DeckSprites[ m_SpriteMap[AssociatedCard.Value - 1][AssociatedCard.Type]]);
    }
    public Sprite GetCardBack()
    {
        return (CardBack);
    }
    public Card DrawCard()
    {
        Card ReturnValue = m_CurrentDeck[m_CurrentDeck.Count - 1];
        m_CurrentDeck.RemoveAt(m_CurrentDeck.Count - 1);
        return (ReturnValue);
    } 
    // Update is called once per frame
    void Update()
    {
        
    }
}
