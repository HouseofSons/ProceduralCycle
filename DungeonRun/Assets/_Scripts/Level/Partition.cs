//This script is attached to a Partition Game Object
//On Start up will catalogue all parameters for Path Finding from the Partition Game Object
    //These parameters are used for Player Path Finding
using UnityEngine;

public class Partition : MonoBehaviour
{
    public Room PartRoom { private set; get; }

    public Vector3Int Origin { private set; get; }
    public int Width { private set; get; }
    public int Depth { private set; get; }

    public int Nedge { private set; get; }
    public int Eedge { private set; get; }
    public int Sedge { private set; get; }
    public int Wedge { private set; get; }

    public int WidthStart { private set; get; }
    public int WidthEnd { private set; get; }
    public int DepthStart { private set; get; }
    public int DepthEnd { private set; get; }

    void Start()
    {
        PartRoom = this.transform.parent.GetComponent<Room>();

        Origin = new Vector3Int(
            Mathf.RoundToInt(this.transform.position.x),
            Mathf.RoundToInt(this.transform.position.y),
            Mathf.RoundToInt(this.transform.position.z));
        Width = Mathf.RoundToInt(this.transform.lossyScale.x);
        Depth = Mathf.RoundToInt(this.transform.lossyScale.z);

        Nedge = Mathf.RoundToInt(Origin.z + (Depth / 2));
        Eedge = Mathf.RoundToInt(Origin.x + (Width / 2));
        Sedge = Mathf.RoundToInt(Origin.z - (Depth / 2));
        Wedge = Mathf.RoundToInt(Origin.x - (Width / 2));
    }
}
