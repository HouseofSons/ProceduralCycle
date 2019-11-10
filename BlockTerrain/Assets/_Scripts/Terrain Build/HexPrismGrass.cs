using UnityEngine;
using System.Collections;

public class HexPrismGrass : HexPrism {

    public HexPrismGrass() : base() {
    }

    public override Vector2 TexturePosition(Direction direction)
    {
        if (direction == Direction.up)
        {
            return new Vector2(0, 0);
        } else if (direction == Direction.down)
        {
            return new Vector2(1, 0);
        }
        else
        {
            return new Vector2(1, 0);
        }

    }
}
