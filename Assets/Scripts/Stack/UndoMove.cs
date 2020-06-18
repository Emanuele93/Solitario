using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class UndoMove
{
    public int points;
    public virtual int Undo() { return 0; }
}

public class UndoSeparateCards : UndoMove
{
    private Card card, cardFrom;
    private bool flipCardFrom;

    public UndoSeparateCards(Card card, Card cardFrom, int points, bool flipCardFrom = false)
    {
        this.card = card;
        this.cardFrom = cardFrom;
        this.points = points;
        this.flipCardFrom = flipCardFrom;
    }

    public override int Undo()
    {
        cardFrom.stack.UndoPlayerMove(card, cardFrom, flipCardFrom);
        return points;
    }
}

public class UndoLeaveStack : UndoMove
{
    Card card;
    Stack stackFrom;

    public UndoLeaveStack(Card card, Stack stackFrom, int points)
    {
        this.card = card;
        this.stackFrom = stackFrom;
        this.points = points;
    }

    public override int Undo()
    {
        stackFrom.UndoPlayerMove(card, null, false);
        return points;
    }
}

public class UndoClickStack : UndoMove
{
    Stack stackFrom;

    public UndoClickStack(Stack stackFrom, int points)
    {
        this.stackFrom = stackFrom;
        this.points = points;
    }

    public override int Undo()
    {
        stackFrom.UndoPlayerMove(null, null, false);
        return points;
    }
}