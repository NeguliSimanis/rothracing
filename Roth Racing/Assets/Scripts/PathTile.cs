using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTile : MonoBehaviour
{
    TileManager tileManager;

    private void Start()
    {
        tileManager = GameManager.instance.gameObject.GetComponent<TileManager>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && GameManager.instance.currentGameState == GameState.Racing)
        {
            tileManager.CollectPathTile();
        }
    }
}
