using UnityEngine;
using System.Collections;

public class HexPrismStone : HexPrism {
	
	public HexPrismStone() : base() {
	}
	
	public override Vector2 TexturePosition(Direction direction)
	{
        return new Vector2(0,0);
	}
}
