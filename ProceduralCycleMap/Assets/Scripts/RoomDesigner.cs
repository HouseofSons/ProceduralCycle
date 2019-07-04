using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDesigner : MonoBehaviour
{
    public static RoomDesigner instance;

    public GameObject stairs;

    public static GameObject Stairs
    {
        get { return instance.stairs; }
    }

    private void Awake()
    {
        instance = this;
    }

    public static void DesignRoom(Room r)
    {
       PlaceStairs(r);
       PlaceDoors(r);
    }

    private static void PlaceStairs(Room r)
    {
        List<int> positions = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 };
        Seed.Shuffle(positions);
        FloorTile firstFloorTile;
        FloorTile secondFloorTile;
        bool positionFound;
        Vector3 position;

        foreach (Door d in r.Doors)
        {
            positionFound = false;

            //identify above neighbor
            if (d.RoomFirstLocation.y < d.RoomSecondLocation.y)
            {
                firstFloorTile = r.GetFloorTile(d.RoomFirstLocation);
                secondFloorTile = d.RoomSecond.GetFloorTile(d.RoomSecondLocation);
                
                //try each position to check for fit
                foreach (int i in positions)
                {
                    if (!positionFound)
                    {
                        switch (i)
                        {
                            case 0:
                                if (secondFloorTile.FloorSpace(9, 14, 1, 1, 1, 3) &&
                                    firstFloorTile.FloorSpace(1, 11, 1, 16, 1, 3) &&
                                    firstFloorTile.FloorSpace(1, 3, 1, 8, 1, 11))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(9, 14, 1, 1, 1, 3);
                                    firstFloorTile.AllocateFloorSpace(1, 11, 1, 16, 1, 3);
                                    firstFloorTile.AllocateFloorSpace(1, 3, 1, 8, 1, 11);
                                    //place stairs on first floor tile
                                    //create floor opening on second floor tile

                                    position = d.RoomFirstLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + (11 / 2f) - 8, position.y + (8 / 2f) - 8, position.z + (3 / 2f) - 8);
                                    GameObject go = Instantiate(Stairs, position, Quaternion.identity);
                                    go.transform.Rotate(0, 0, 0, Space.Self);
                                }

                                break;
                            case 1:
                                if (secondFloorTile.FloorSpace(3, 8, 1, 1, 1, 3) &&
                                    firstFloorTile.FloorSpace(5, 16, 1, 16, 1, 3) &&
                                    firstFloorTile.FloorSpace(14, 16, 1, 8, 1, 11))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(3, 8, 1, 1, 1, 3);
                                    firstFloorTile.AllocateFloorSpace(5, 16, 1, 16, 1, 3);
                                    firstFloorTile.AllocateFloorSpace(14, 16, 1, 8, 1, 11);
                                    //place stairs on first floor tile
                                    //create floor opening on second floor tile
                                }

                                break;
                            case 2:
                                if (secondFloorTile.FloorSpace(9, 14, 1, 1, 14, 16) &&
                                    firstFloorTile.FloorSpace(1, 11, 1, 16, 14, 16) &&
                                    firstFloorTile.FloorSpace(1, 3, 1, 8, 6, 16))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(9, 14, 1, 1, 14, 16);
                                    firstFloorTile.AllocateFloorSpace(1, 11, 1, 16, 14, 16);
                                    firstFloorTile.AllocateFloorSpace(1, 3, 1, 8, 6, 16);
                                    //place stairs on first floor tile
                                    //create floor opening on second floor tile
                                }

                                break;
                            case 3:
                                if (secondFloorTile.FloorSpace(3, 8, 1, 1, 14, 16) &&
                                    firstFloorTile.FloorSpace(5, 16, 1, 16, 14, 16) &&
                                    firstFloorTile.FloorSpace(14, 16, 1, 8, 6, 16))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(3, 8, 1, 1, 14, 16);
                                    firstFloorTile.AllocateFloorSpace(5, 16, 1, 16, 14, 16);
                                    firstFloorTile.AllocateFloorSpace(14, 16, 1, 8, 6, 16);
                                    //place stairs on first floor tile
                                    //create floor opening on second floor tile
                                }

                                break;
                            case 4:
                                if (secondFloorTile.FloorSpace(1, 3, 1, 1, 9, 14) &&
                                    firstFloorTile.FloorSpace(1, 3, 1, 16, 1, 11) &&
                                    firstFloorTile.FloorSpace(1, 11, 1, 8, 1, 3))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(1, 3, 1, 1, 9, 14);
                                    firstFloorTile.AllocateFloorSpace(1, 3, 1, 16, 1, 11);
                                    firstFloorTile.AllocateFloorSpace(1, 11, 1, 8, 1, 3);
                                    //place stairs on first floor tile
                                    //create floor opening on second floor tile
                                }

                                break;
                            case 5:
                                if (secondFloorTile.FloorSpace(1, 3, 1, 1, 3, 8) &&
                                    firstFloorTile.FloorSpace(1, 3, 1, 16, 5, 16) &&
                                    firstFloorTile.FloorSpace(1, 11, 1, 8, 14, 16))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(1, 3, 1, 1, 3, 8);
                                    firstFloorTile.AllocateFloorSpace(1, 3, 1, 16, 5, 16);
                                    firstFloorTile.AllocateFloorSpace(1, 11, 1, 8, 14, 16);
                                    //place stairs on first floor tile
                                    //create floor opening on second floor tile
                                }

                                break;
                            case 6:
                                if (secondFloorTile.FloorSpace(14, 16, 1, 1, 9, 14) &&
                                    firstFloorTile.FloorSpace(14, 16, 1, 16, 1, 11) &&
                                    firstFloorTile.FloorSpace(6, 16, 1, 8, 1, 3))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(14, 16, 1, 1, 9, 14);
                                    firstFloorTile.AllocateFloorSpace(14, 16, 1, 16, 1, 11);
                                    firstFloorTile.AllocateFloorSpace(6, 16, 1, 8, 1, 3);
                                    //place stairs on first floor tile
                                    //create floor opening on second floor tile
                                }

                                break;
                            default:
                                if (secondFloorTile.FloorSpace(14, 16, 1, 1, 3, 8) &&
                                    firstFloorTile.FloorSpace(14, 16, 1, 16, 5, 16) &&
                                    firstFloorTile.FloorSpace(6, 16, 1, 8, 14, 16))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(14, 16, 1, 1, 3, 8);
                                    firstFloorTile.AllocateFloorSpace(14, 16, 1, 16, 5, 16);
                                    firstFloorTile.AllocateFloorSpace(6, 16, 1, 8, 14, 16);
                                    //place stairs on first floor tile
                                    //create floor opening on second floor tile
                                }

                                break;
                        }
                    }
                }
            }
        }
    }

    private static void PlaceDoors(Room r)
    {

    }
}
