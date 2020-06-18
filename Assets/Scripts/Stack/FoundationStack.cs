using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoundationStack : Stack
{
    public Suit suit;

    public bool Completed()
    {
        return (cards.Count == 13);
    }

    public override bool ConnectCard(Card card, Card stackCard = null)
    {
        if (cards.Count == 0 && card.value == 1 && card.suit == suit)
        {
            GameManager.Instance.AddUndoMove(CreateUndoMove(card));
            ConnectCardOperations(card);
            card.cardDown = null;
            return true;
        }
        else if(stackCard != null && stackCard.cardUp == null && stackCard.value == card.value - 1 && stackCard.suit == card.suit && card.cardUp == null)
        {
            GameManager.Instance.AddUndoMove(CreateUndoMove(card));
            ConnectCardOperations(card);
            stackCard.cardUp = card;
            card.cardDown = stackCard;
            return true;
        }
        return false;
    }

    public override void UndoPlayerMove(Card oldCardUp, Card cardDown, bool flipCardDown)
    {
        if (oldCardUp.cardDown != null)
            oldCardUp.cardDown.cardUp = null;

        ConnectCardOperations(oldCardUp);
        if (cardDown == null)
            oldCardUp.cardDown = null;
        else
        {
            cardDown.cardUp = oldCardUp;
            oldCardUp.cardDown = cardDown;
        }
    }

    private UndoMove CreateUndoMove(Card card)
    {
        UndoMove action;
        if (card.cardDown != null)
        {
            if (card.cardDown.GetComponent<BoxCollider2D>().enabled)
                action = new UndoSeparateCards(card, card.cardDown, addCardPoints);
            else
            {
                action = new UndoSeparateCards(card, card.cardDown, addCardPoints + flipCardPoints, true);
                card.cardDown.SetDragDrop(true);
                card.cardDown.GetComponent<Animator>().Play("BackToFront");
            }
            card.cardDown.cardUp = null;
        }
        else
            action = new UndoLeaveStack(card, card.stack, addCardPoints);
        return action;
    }

    private void ConnectCardOperations(Card card, Card stackCard = null)
    {
        card.SetNewPositionCascade(new Vector3(transform.position.x, transform.position.y, transform.position.z - 1 - cards.Count));
        card.stack.RemoveCard(card);
        card.transform.parent = transform;
        card.stack = this;
        cards.Add(card.gameObject);
        if (Completed())
            GameManager.Instance.EndGame();
    }
}
