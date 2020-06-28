using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Card : MonoBehaviour
{
    public int value;                   // TODO private con get
    public Suit suit;                   // TODO private con get
    public Color color;                 // TODO private con get
    public Stack stack;                 // TODO private con get
    public Card cardUp, cardDown;     // TODO private con get e set

    public SpriteRenderer suitSprite, suitSmallSprite, numberSprite;

    public float movementSpeed;

    private CardAction action;
    private Vector3 oldPos, startDiff;

    public void Create(int value, Suit suit, Sprite suitSprite, Sprite numberSprite)
    {
        this.value = value;
        this.suit = suit;
        this.suitSprite.sprite = suitSprite;
        this.suitSmallSprite.sprite = suitSprite;
        this.color = suit == Suit.Clovers || suit == Suit.Pikes ? Color.black : Color.red;
        this.numberSprite.sprite = numberSprite;
        this.numberSprite.color = color;
        action = new NoAction();
    }

    public void SetNoAction()
    {
        action = new NoAction();
    }

    public void SetNewPosition(Vector3 pos, bool endFlip = false)
    {
        action = new MoveCard(pos, movementSpeed, gameObject, endFlip);
        oldPos = pos;
    }

    public void SetNewPositionCascade(Vector3 pos)
    {
        Vector3 dif;
        if (cardUp != null)
        {
            dif = cardUp.oldPos - oldPos;
            action = new MoveCard(pos, movementSpeed, gameObject);
            oldPos = pos;
            cardUp.SetNewPositionCascade(pos + dif);
        }
        else
        {
            action = new MoveCard(pos, movementSpeed, gameObject);
            oldPos = pos;
        }
    }

    private void SetPositionCascade(Vector3 pos)
    {
        Vector3 dif;
        if (cardUp != null)
        {
            dif = cardUp.transform.position - transform.position;
            transform.position = pos;
            cardUp.SetPositionCascade(pos + dif);
        }
        else
            transform.position = pos;
    }

    void Update()
    {
        action.Action();
    }

    public void SetDragDrop(bool val)
    {
        GetComponent<BoxCollider2D>().enabled = val;
    }

    void OnMouseDrag()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - startDiff;
        SetPositionCascade(new Vector3(pos.x, pos.y, 0));
    }

    private float doubleClickStart = 0;

    private void OnMouseDown()
    {
        if ((Time.time - doubleClickStart) > 0.3f && action.GetType().Name == "NoAction")
            startDiff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
    }

    void OnMouseUp()
    {
        if ((Time.time - doubleClickStart) <= 0.3f)
        {
            if (!GameManager.Instance.GetHelp(this))
                SetNewPositionCascade(oldPos);
            doubleClickStart = -1;
        }
        else
        {
            doubleClickStart = Time.time;

            ChangeColliderCascade(false);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero);
            if (!hit)
                hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            ChangeColliderCascade(true);
            if (hit)
            {
                Card cardDown = hit.collider.GetComponent<Card>();
                if (cardDown != null && cardDown.cardUp == null && cardDown.stack.ConnectCard(this, cardDown))
                    return;
                else
                {
                    Stack stack = hit.collider.GetComponent<Stack>();
                    if (stack != null && stack.ConnectCard(this))
                        return;
                }
            }

            SetNewPositionCascade(oldPos);
        }
    }

    protected void ChangeColliderCascade(bool val)
    {
        GetComponent<Collider2D>().enabled = val;
        if (cardUp != null)
            cardUp.ChangeColliderCascade(val);

    }
}