using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteTiles
{
    public static List<Vector2> RandomTileUVs(string theme)
    {
        List<Vector2> uvs = new List<Vector2>();

        if (theme == "DungeonFloor")
        {   //256 pixel sprite 16x16 cells
            int rand = Random.Range(0, 100);

            if (rand < 20)                      { rand = 5; }
            else if (rand < 40)                 { rand = 5; }
            else if (rand < 60)                 { rand = 5; }
            else if (rand < 80)                 { rand = 5; }
            else if (rand < 82)                 { rand = 6; }
            else if (rand < 84)                 { rand = 7; }
            else if (rand < 86)                 { rand = 8; }
            else if (rand < 88)                 { rand = 9; }
            else if (rand < 90)                 { rand = 10; }
            else if (rand < 92)                 { rand = 11; }
            else if (rand < 94)                 { rand = 12; }
            else                                { rand = 0; }

            uvs.Add(new Vector2(rand / 16.0f, 15 / 16.0f));
            uvs.Add(new Vector2(rand / 16.0f, 16 / 16.0f));
            uvs.Add(new Vector2((rand + 1) / 16.0f, 16 / 16.0f));
            uvs.Add(new Vector2((rand + 1) / 16.0f, 15 / 16.0f));
        }
        else
        {
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 0));
        }
        return uvs;
    }
}
