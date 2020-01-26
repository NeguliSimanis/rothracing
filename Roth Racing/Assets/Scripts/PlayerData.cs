using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public static PlayerData instance;

    #region MOVEMENT
    public bool canMove = true;
    public float moveSpeed = 140f;
    #endregion
}
