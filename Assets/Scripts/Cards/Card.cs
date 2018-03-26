﻿using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

public enum CardTypes
{
    PATH_CARD,
    DESTROY_PATH_CARD,
    SWAP_PATH_CARD,
    ROTATE_PATH_CARD,
    MOVE_PATH_CARD
}

[System.Serializable]
public struct CardEditor
{
    public Text Description;
    public Image CardBackground;
    public Image CardImage;
}

[System.Serializable]
public class PathCardData
{
    public PathCardData(bool Up, bool Down, bool Left, bool Right, bool Middle)
    {
        this.Up = Up;
        this.Down = Down;
        this.Left = Left;
        this.Right = Right;
        this.Middle = Middle;
    }

    public bool Up { get; set; }
    public bool Down { get; set; }
    public bool Left { get; set; }
    public bool Right { get; set; }
    public bool Middle { get; set; }
}

[System.Serializable]
public class CardData
{
    public CardData(CardTypes Type, string Description, Sprite CardSprite, PathCardData PathData)
    {
        this.Type = Type;
        this.Description = Description;
        this.CardSprite = CardSprite;
        this.PathData = PathData;
    }

    public CardTypes Type;
    public string Description;
    public Sprite CardSprite;
    public PathCardData PathData;
}

public class Card : MonoBehaviour
{
    protected CardData m_CardData;
    public CardData CardData { get { return m_CardData; } set { m_CardData = value; } }
    [SerializeField]protected CardEditor m_CardEditor;
    public CardEditor CardEditor { get { return m_CardEditor; } set { m_CardEditor = value; } }

    [SerializeField] private SkeletonGraphic m_Shine;
    [SerializeField] private SkeletonGraphic m_Burn;

    private int m_IndexInHand;
    private Vector2 m_DefaultPosition;
    public int IndexInHand { get { return m_IndexInHand; } }
    public Vector2 DefaultPosition { get { return m_DefaultPosition; }}

    private void Awake()
    {
        RectTransform card = transform as RectTransform;
        m_IndexInHand = transform.GetSiblingIndex(); // Sets the hierarchy layer
        m_DefaultPosition = card.anchoredPosition; // Sets default position in UI
    }

    public void SetCardInfo()
    {
        //Shows the cards info
        m_CardEditor.Description.text = m_CardData.Description;
        m_CardEditor.CardImage.sprite = m_CardData.CardSprite;
        if(m_CardData.Type == CardTypes.PATH_CARD)
        {
            m_CardEditor.CardBackground.sprite = CardImageLoader.s_CardBackgroundSprite(CardStrings.PATH_BACKGROUND);
        }
        else
        {
            m_CardEditor.CardBackground.sprite = CardImageLoader.s_CardBackgroundSprite(CardStrings.ACTION_BACKGROUND);
        }
        m_CardEditor.CardBackground.color = TurnManager.s_Instance.CurrentPlayer.PlayerData.PlayerColor;
    }

    public void ToggleCardSelect()
    {
        CardSelector.s_OnToggleCardSelect(this,m_IndexInHand);
    }

    public void Shine()
    {
        Debug.Log("Shine Card");
        m_Shine.AnimationState.SetAnimation(0, "animation", false);
    }

    public void Burn()
    {
        Debug.Log("Burn Card");
        m_Burn.AnimationState.SetAnimation(0, "animation", false);
    }
}
