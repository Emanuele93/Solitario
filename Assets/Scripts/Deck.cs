using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public Sprite[] suit_sprites;
    public Sprite[] value_sprites;
    public GameObject cardPrefab;
    public WasteStack wasteStack;

    private List<GameObject> cards = new List<GameObject>();
    private List<Suit> suits = new List<Suit>() { Suit.Hearts, Suit.Clovers, Suit.Pikes, Suit.Tiles };
    private GameObject card;

    public void CreateCard(int suit, int value)
    {
        card = Instantiate(cardPrefab, transform.position, Quaternion.identity, transform);
        card.name = value + "-" + suits[suit];
        card.GetComponent<Card>().Create(value, suits[suit], suit_sprites[suit], value_sprites[value - 1]);
        cards.Add(card);
    }

    public void Shuffle()
    {
        int n = cards.Count;
        Vector3 v;
        while (n > 1)
        {
            int k = Random.Range(0, n);
            n--;
            card = cards[k];
            cards[k] = cards[n];
            cards[n] = card;
        }

        for (int i = cards.Count - 1; i >= 0; i--)
            cards[i].transform.position = new Vector3(transform.position.x, transform.position.y, cards.Count - i);
    }

    public Card GetCard()
    {
        if (cards.Count == 0)
            return null;

        card = cards[cards.Count - 1]; 
        cards.Remove(card);
        card.transform.position = new Vector3(card.transform.position.x, card.transform.position.y, 0);
        return card.GetComponent<Card>();
    }

    public void AddCard(Card card)
    {
        cards.Add(card.gameObject);
        card.SetNewPosition(new Vector3(transform.position.x, transform.position.y, transform.position.z + cards.Count));
        card.GetComponent<Animator>().Play("FrontToBack");
        card.SetDragDrop(false);
        card.transform.parent = transform;
        card.stack.RemoveCard(card);
    }

    private void OnMouseUp()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit && hit.collider.gameObject == gameObject)
        {
            if (cards.Count == 0 && wasteStack.CountCards() == 0)
                return;

            if (cards.Count == 0)
            {
                GameManager.Instance.AddUndoMove(new UndoLeaveStack(null, wasteStack, -100));
                wasteStack.RemoveAllCards();
            }
            else
            {
                GameManager.Instance.AddUndoMove(new UndoLeaveStack(null, wasteStack, 0));
                Card card = GetCard();
                card.GetComponent<Animator>().Play("BackToFront");
                card.SetDragDrop(true);
                wasteStack.AddCard(card);
            }
        }
    }

    public void Reset()
    {
        cards.Clear();
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        wasteStack.Reset();
    }
}
