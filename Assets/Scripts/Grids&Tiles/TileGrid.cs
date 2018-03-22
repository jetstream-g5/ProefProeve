﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{

    public static TileGrid s_Instance;

    [SerializeField] private int m_GridXSize = 10;
    [SerializeField] private int m_GridYSize = 10;

    [SerializeField] private GameObject m_NodeObjectPrefab;

    private TileNode[,] m_NodeGrid;
    private GameObject[,] m_NodeBridges;


    private TileNode[] m_PlayerStartNodes;
    public List<TileNode> NodeRoad { get; set; }

    public List<TileNode> CheckedNodes { get; set; }
    public bool RoadCompleted { get; set; }

    #region startUp
    private void Awake()
    {
        Init();
    }

    //make instance
    private void Init()
    {
        if (s_Instance == null)
            s_Instance = this;
        else
            Destroy(gameObject);
    }


    // Use this for initialization
    void Start()
    {
        InstantiateNewGrid();
        SetNeighboursOfNodes();
    }
    #endregion

    #region void functions
    public void InstantiateNewGrid()
    {
        int amountOfPlayersPlaceholder = 4;
        Debug.LogError("the amount of players still needs to be recieved");
        m_PlayerStartNodes = new TileNode[amountOfPlayersPlaceholder];
        m_NodeBridges = new GameObject[m_GridXSize, m_GridYSize];
        m_NodeGrid = new TileNode[m_GridXSize + 2, m_GridYSize + 2];
        for (int i = 0; i < m_GridXSize + 2; i++)
        {
            for (int j = 0; j < m_GridYSize + 2; j++)
            {
                TileNode newNode = new TileNode();
                TileBools bools = new TileBools();
                newNode.IsFilled = false;
                newNode.IsDestructable = true;
                if (i == 0 || i == m_GridXSize + 1 || j == 0 || j == m_GridYSize + 1)
                {
                    newNode.IsEdgeStone = true;
                    newNode.IsDestructable = false;
                    //GameObject tileObj = Instantiate(new GameObject(), new Vector3(i, j, 0), Quaternion.identity);
                    newNode.TileObject = null;
                }
                else
                {
                    GameObject tileObj = Instantiate(m_NodeObjectPrefab, new Vector3(i, j, 0), Quaternion.identity, transform);
                    ClickedOnTile TileClicker = tileObj.GetComponent<ClickedOnTile>();
                    TileClicker.TilePosX = i;
                    TileClicker.TilePosY = j;
                    newNode.TileObject = tileObj;
                }
                if (i == 1 && j == 1)
                {
                    newNode.IsFilled = true;
                    newNode.IsDestructable = false;
                    bools.Right = true;
                    bools.Up = true;
                    bools.Middle = true;
                    newNode.IsFilled = true;
                    newNode.IsStartPoint = true;
                    m_PlayerStartNodes[2] = newNode;
                }
                else if (i == m_GridXSize && j == 1)
                {
                    newNode.IsFilled = true;
                    newNode.IsDestructable = false;
                    bools.Left = true;
                    bools.Up = true;
                    bools.Middle = true;
                    newNode.IsFilled = true;
                    newNode.IsStartPoint = true;
                    m_PlayerStartNodes[1] = newNode;
                }
                else if (i == 1 && j == m_GridYSize)
                {
                    newNode.IsFilled = true;
                    newNode.IsDestructable = false;
                    bools.Right = true;
                    bools.Down = true;
                    bools.Middle = true;
                    newNode.IsFilled = true;
                    newNode.IsStartPoint = true;
                    m_PlayerStartNodes[0] = newNode;
                }
                else if (i == m_GridXSize && j == m_GridYSize)
                {
                    newNode.IsFilled = true;
                    newNode.IsDestructable = false;
                    bools.Left = true;
                    bools.Down = true;
                    bools.Middle = true;
                    newNode.IsFilled = true;
                    newNode.IsStartPoint = true;
                    m_PlayerStartNodes[3] = newNode;
                }
                else if (i == 5 && j == 5)
                {
                    newNode.IsFilled = true;
                    newNode.IsDestructable = false;
                    bools.Left = true;
                    bools.Up = true;
                    bools.Middle = true;
                    bools.Down = true;
                    bools.Right = true;
                    newNode.IsFilled = true;
                    newNode.IsEndpoint = true;
                }
                newNode.Bools = bools;
                newNode.UpdateArt();
                m_NodeGrid[i, j] = newNode;
            }
        }
    }

    public void SetAccesableNeighboursOfNodes()
    {
        if (m_NodeGrid == null)
        {
            throw new System.ArgumentException("The NodeGrid cannot be null", "original");
        }
        else
        {
            for (int i = 0; i < m_GridXSize + 2; i++)
            {
                for (int j = 0; j < m_GridYSize + 2; j++)
                {
                    if (!m_NodeGrid[i, j].IsEdgeStone)
                    {
                        m_NodeGrid[i, j].SetAccesableNeighbours();
                    }
                }
            }
        }
    }

    public void SetNeighboursOfNodes()
    {
        if (m_NodeGrid == null)
        {
            throw new System.ArgumentException("The NodeGrid cannot be null", "original");
        }
        else
        {
            for (int i = 0; i < m_GridXSize + 2; i++)
            {
                for (int j = 0; j < m_GridYSize + 2; j++)
                {
                    if (!m_NodeGrid[i, j].IsEdgeStone)
                    {
                        Neighbours neighbours = new Neighbours();
                        neighbours.up = m_NodeGrid[i, j + 1];
                        neighbours.down = m_NodeGrid[i, j - 1];
                        neighbours.right = m_NodeGrid[i + 1, j];
                        neighbours.left = m_NodeGrid[i - 1, j];
                        m_NodeGrid[i, j].AllNeighbours = neighbours;
                    }
                }
            }
        }
    }
    #endregion

    #region return value functions

    public bool MoveNode(int xMovingNode, int yMovingNode, int xTargetNode, int yTargetNode, float delay = 0)
    {
        TileNode MovingNode = m_NodeGrid[xMovingNode, yMovingNode];
        if (PlaceNewCard(xTargetNode, yTargetNode, MovingNode.Bools.Up, MovingNode.Bools.Right, MovingNode.Bools.Down, MovingNode.Bools.Left, MovingNode.Bools.Middle))
        {
            DestroyNode(xMovingNode, yMovingNode, delay);
            return true;
        }
        else
        {
            return false;
        }
    }


    public bool DestroyNode(int x, int y, float delay = 0)
    {
        if (!m_NodeGrid[x, y].IsEdgeStone && m_NodeGrid[x, y].IsFilled && m_NodeGrid[x, y].IsDestructable)
        {
            TileBools bools = new TileBools();
            bools.Up = false;
            bools.Right = false;
            bools.Down = false;
            bools.Left = false;
            bools.Middle = false;
            m_NodeGrid[x, y].Bools = bools;
            m_NodeGrid[x, y].IsFilled = false;

            if(delay>0)
            {
                StartCoroutine(UpdateArt(m_NodeGrid[x, y], delay));
            }
            else
            {
                m_NodeGrid[x, y].UpdateArt();
            }            
            return true;
        }
        else
        {
            return false;
        }
    }
    IEnumerator UpdateArt(TileNode T, float delay)
    {
        yield return new WaitForSeconds(delay);
        T.UpdateArt();
    }

    public bool PlaceNewCard(int x, int y, bool UpCard, bool RightCard, bool DownCard, bool LeftCard, bool MiddleCard, float delay = 0)
    {
        if (m_NodeGrid[x, y].CheckPlacement(UpCard, RightCard, DownCard, LeftCard) && !m_NodeGrid[x, y].IsFilled)
        {
            TileBools bools = new TileBools();
            bools.Up = UpCard;
            bools.Right = RightCard;
            bools.Down = DownCard;
            bools.Left = LeftCard;
            bools.Middle = MiddleCard;
            m_NodeGrid[x, y].Bools = bools;
            m_NodeGrid[x, y].IsFilled = true;
            if(delay > 0)
            {
                StartCoroutine(UpdateArt(m_NodeGrid[x, y], delay));
            }
            else
            {
                m_NodeGrid[x, y].UpdateArt();
            }
            ;
            if (UpCard && m_NodeGrid[x, y].AllNeighbours.up.IsFilled)
            {
                GameObject Bridge = Instantiate(TileArtLib.s_Bridges[0], new Vector3(x , y + 0.5f, 0), Quaternion.identity, m_NodeGrid[x,y].TileObject.transform);
                //m_NodeBridges[] = Bridge;
            }
            if (RightCard && m_NodeGrid[x, y].AllNeighbours.right.IsFilled)
            {
                GameObject Bridge = Instantiate(TileArtLib.s_Bridges[1], new Vector3(x +0.5f, y, 0), Quaternion.identity, m_NodeGrid[x, y].TileObject.transform);
                //m_NodeBridges[] = Bridge;
            }
            if (DownCard && m_NodeGrid[x, y].AllNeighbours.down.IsFilled)
            {
                GameObject Bridge = Instantiate(TileArtLib.s_Bridges[0], new Vector3(x , y- 0.5f, 0), Quaternion.identity, m_NodeGrid[x, y].TileObject.transform);
                //m_NodeBridges[] = Bridge;
            }
            if (LeftCard && m_NodeGrid[x, y].AllNeighbours.left.IsFilled)
            {
                GameObject Bridge = Instantiate(TileArtLib.s_Bridges[1], new Vector3(x - 0.5f, y, 0), Quaternion.identity, m_NodeGrid[x, y].TileObject.transform);
                //m_NodeBridges[] = Bridge;
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CompleteRoad(int startPlayer)
    {
        NodeRoad = new List<TileNode>();
        CheckedNodes = new List<TileNode>();
        m_PlayerStartNodes[startPlayer-1].GetChecked(this, null);
        
        for (int i = 0; i < CheckedNodes.Count; i++)
        {
            CheckedNodes[i].IsChecked = false;
            CheckedNodes[i].UpdateArt();
        }

        return RoadCompleted;
    }
    #endregion
}

public struct Bridges
{

}