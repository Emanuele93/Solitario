using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableauStack : Stack
{
    public float spaceHideCards = 0.3f;
    public float spaceVisibleCards = 0.5f;

    public void AddCard(Card card, bool flip = false)
    {
        card.SetNewPosition(new Vector3(
            transform.position.x, 
            transform.position.y - (cards.Count * spaceHideCards), 
            transform.position.z - 1 - cards.Count), flip);

        if (cards.Count > 0)
        {
            card.cardDown = cards[cards.Count - 1].GetComponent<Card>();
            cards[cards.Count - 1].GetComponent<Card>().cardUp = card;
        }
        AddCardOperations(card);
    }

    public override bool ConnectCard(Card card, Card stackCard = null)
    {
        if (cards.Count == 0 && card.value == 13)
        {
            GameManager.Instance.AddUndoMove(CreateUndoMove(card));
            ConnectCardOperations(card, spaceVisibleCards);
            card.cardDown = null;
            return true;
        }
        else if (stackCard != null && stackCard.cardUp == null && stackCard.value == card.value + 1 && stackCard.color != card.color)
        {
            GameManager.Instance.AddUndoMove(CreateUndoMove(card));
            ConnectCardOperations(card, spaceVisibleCards);
            stackCard.cardUp = card;
            card.cardDown = stackCard;
            return true;
        }
        return false;
    }

    public override void UndoPlayerMove(Card oldCardUp, Card cardDown, bool flipCardDown)
    {
        if (flipCardDown)
        {
            cardDown.SetDragDrop(false);
            cardDown.GetComponent<Animator>().Play("FrontToBack");
        }
        if (oldCardUp.cardDown != null)
            oldCardUp.cardDown.cardUp = null;

        ConnectCardOperations(oldCardUp, spaceHideCards);
        if (cardDown == null)
            oldCardUp.cardDown = null;
        else
        {
            cardDown.cardUp = oldCardUp;
            oldCardUp.cardDown = cardDown;
        }
    }

    private void ConnectCardOperations(Card card, float ySpace)
    {
        card.SetNewPositionCascade(new Vector3(transform.position.x,
            cards.Count == 0 ? transform.position.y : (cards[cards.Count - 1].transform.position.y - ySpace), 
            transform.position.z - 1 - cards.Count));
        card.stack.RemoveCard(card);
        AddCardOperations(card);
        if (card.cardUp != null)
            AddCardCascade(card.cardUp);
    }

    private void AddCardCascade(Card card)
    {
        card.stack.RemoveCard(card);
        AddCardOperations(card);
        if (card.cardUp != null)
            AddCardCascade(card.cardUp);
    }

    private void AddCardOperations(Card card)
    {
        card.transform.parent = transform;
        card.stack = this;
        cards.Add(card.gameObject);
    }

    private UndoMove CreateUndoMove(Card card)
    {
        UndoMove action;
        if (card.cardDown != null)
        {
            if (card.cardDown.GetComponent<BoxCollider2D>().enabled)
                action = new UndoSeparateCards(card, card.cardDown, card.stack.removeCardPoints);
            else
            {
                action = new UndoSeparateCards(card, card.cardDown, card.stack.removeCardPoints + flipCardPoints, true);
                card.cardDown.SetDragDrop(true);
                card.cardDown.GetComponent<Animator>().Play("BackToFront");
            }
            card.cardDown.cardUp = null;
        }
        else
            action = new UndoLeaveStack(card, card.stack, card.stack.extraPoints + card.stack.removeCardPoints);
        return action;
    }
}
