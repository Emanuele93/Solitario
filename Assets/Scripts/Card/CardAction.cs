using UnityEngine;

public interface CardAction
{
    void Action();
}

public class NoAction : CardAction
{
    public void Action() { }
}

public class MoveCard : CardAction
{
    private Vector3 newPosition, oldPosition, dir;
    private GameObject cardObj;
    private Card card;
    private float movementSpeed;
    private bool endFlip;

    public MoveCard(Vector3 newPosition, float movementSpeed, GameObject card, bool endFlip = false)
    {
        this.newPosition = newPosition;
        this.movementSpeed = movementSpeed;
        this.cardObj = card;
        this.endFlip = endFlip;
        this.card = card.GetComponent<Card>();
    }

    public void Action()
    {
        oldPosition = cardObj.transform.position;
        dir = (newPosition - cardObj.transform.position).normalized;
        card.transform.position = cardObj.transform.position + dir * movementSpeed;
        if ((newPosition - oldPosition).sqrMagnitude < (cardObj.transform.position - oldPosition).sqrMagnitude)
        {
            card.transform.position = newPosition;
            if (endFlip)
            {
                card.SetDragDrop(true);
                cardObj.GetComponent<Animator>().Play("BackToFront");
            }
            card.SetNoAction();
        }
    }
}

public class MoveCardCascade : CardAction
{
    private Vector3 newPosition, oldPosition, dir;
    private GameObject cardObj;
    private Card card;
    private float movementSpeed;

    public MoveCardCascade(Vector3 newPosition, float movementSpeed, GameObject card)
    {
        this.newPosition = newPosition;
        this.movementSpeed = movementSpeed;
        this.cardObj = card;
        this.card = card.GetComponent<Card>();
    }

    public void Action()
    {
        oldPosition = cardObj.transform.position;
        dir = (newPosition - cardObj.transform.position).normalized;
        card.SetPositionCascade(cardObj.transform.position + dir * movementSpeed);
        if ((newPosition - oldPosition).sqrMagnitude < (cardObj.transform.position - oldPosition).sqrMagnitude)
        {
            card.SetPositionCascade(newPosition);
            card.SetNoAction();
        }
    }
}