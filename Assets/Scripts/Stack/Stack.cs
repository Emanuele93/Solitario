using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Stack : MonoBehaviour
{
    protected List<GameObject> cards = new List<GameObject>();
    public int flipCardPoints = 5;
    public int addCardPoints = 0;
    public int removeCardPoints = 0;
    public int extraPoints = 0;

    public virtual bool ConnectCard(Card card, Card stackCard = null) 
    { 
        return false; 
    }

    public virtual void RemoveCard(Card card)
    {
        cards.Remove(card.gameObject);
    }

    public virtual void UndoPlayerMove(Card oldCardUp, Card cardDown, bool flipCardDown) 
    { 
    
    }

    public bool HeplConnectCard(Card card)
    {
        return ConnectCard(card, cards.Count > 0 ? cards[cards.Count - 1].GetComponent<Card>() : null);
    }

    public void Reset()
    {
        cards.Clear();
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }
}
