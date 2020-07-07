using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteTiles
{
    public static Vector2[] RandomTileUVs(string theme)
    {
        if (theme == "DungeonFloor")
        {   //256 pixel sprite 16x16 cells
            int rand = Random.Range(0, 100);

            if (rand <= 96)
            {
                rand = 0;
            } else if (rand <= 97)
            {
                rand = 1;
            } else if (rand <= 98)
            {
                rand = 2;
            } else
            {
                rand = 3;
            }

            return new Vector2[]
            {
                new Vector2(rand/16.0f,15/16.0f),
                new Vector2(rand/16.0f,16/16.0f),
                new Vector2((rand+1)/16.0f,16/16.0f),
                new Vector2((rand+1)/16.0f,15/16.0f)
            };
        }
        else
        {
            return new Vector2[]
            {
                new Vector2(0,0),
                new Vector2(0,1),
                new Vector2(1,1),
                new Vector2(1,0)
            };
        }
    }
}
