using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WasteStack : Stack
{
    public float spaceBetweenCards = 0.4f;
    public Deck deck;

    public int CountCards()
    {
        return cards.Count;
    }

    public void AddCard(Card card)
    {
        card.transform.parent = transform;
        card.stack = this;
        int count = cards.Count;
        card.SetNewPosition(new Vector3(
            transform.position.x + (count == 1 ? spaceBetweenCards : (count > 1 ? 2 * spaceBetweenCards : 0)), 
            transform.position.y, transform.position.z - 1 - count), false);

        for (int i = count - 1; i >= 0; i--)
        {
            cards[i].GetComponent<Card>().SetNewPosition(new Vector3(
                transform.position.x + (count > 1 && i == count - 1 ? spaceBetweenCards : 0),
                transform.position.y, transform.position.z - i));
            cards[i].GetComponent<BoxCollider2D>().enabled = false;
        }
        cards.Add(card.gameObject);
    }

    public override void RemoveCard(Card card)
    {
        cards.Remove(card.gameObject);
        int count = cards.Count;
        for (int i = count - 1; i >= 0; i--)
            cards[i].GetComponent<Card>().SetNewPosition(new Vector3(
                transform.position.x + ((count > 2 && i == count - 2 || count == 2 && i == 1) ? spaceBetweenCards : (count > 2 && i == count - 1 ? 2f * spaceBetweenCards : 0)),
                transform.position.y, transform.position.z - i)); 
        if (count > 0)
            cards[count - 1].GetComponent<BoxCollider2D>().enabled = true;
    }

    public override void UndoPlayerMove(Card oldCardUp, Card cardDown, bool flipCardDown)
    {
        if (oldCardUp == null)
        {
            if (cards.Count == 0)
            {
                Card card = deck.GetCard();
                while (card != null)
                {
                    card.GetComponent<Animator>().Play("BackToFront");
                    card.SetDragDrop(true);
                    AddCard(card);
                    card = deck.GetCard();
                }

            }
            else
                deck.AddCard(cards[cards.Count - 1].GetComponent<Card>());
        }
        else
        {
            if (oldCardUp.cardDown != null)
                oldCardUp.cardDown.cardUp = null;

            oldCardUp.cardUp = null;
            oldCardUp.cardDown = null;
            oldCardUp.stack.RemoveCard(oldCardUp);

            AddCard(oldCardUp);
        }
    }

    public void RemoveAllCards()
    {
        while (cards.Count > 0)
            deck.AddCard(cards[cards.Count - 1].GetComponent<Card>());
    }
}
