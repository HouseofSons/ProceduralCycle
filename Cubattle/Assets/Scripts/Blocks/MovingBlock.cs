using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBlock : Block
{
    protected override void Start()
    {
        Blocks.Add(this);
        //Not attached to specific grid location
        //MapGrid.InitializeGridLocation(this);
    }
}
