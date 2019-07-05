using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDesigner : MonoBehaviour
{
    public static RoomDesigner instance;
    public GameObject stairs;
    public GameObject platform;

    public static GameObject Stairs { get { return instance.stairs; } }
    public static GameObject Platform { get { return instance.platform; } }

    private void Awake()
    {
        instance = this;
    }

    public static void PlaceStairs(Room r)
    {
        List<int> positions = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 };
        Seed.Shuffle(positions);
        FloorTile firstFloorTile;
        FloorTile secondFloorTile;
        bool positionFound;
        Vector3 position;

        foreach (Door d in r.Doors)
        {
            //Debug.Log("Room: " + r.Order + " Door1: " + d.RoomFirst.Order + " @ " + d.RoomFirstLocation + " Door2: " + d.RoomSecond.Order + " @ " + d.RoomSecondLocation);
            positionFound = false;

            Vector3Int StairsLocation;

            //identify above neighbor
            if ((d.RoomSecondLocation.y - d.RoomFirstLocation.y == 1 && d.RoomFirst == r) ||
                (d.RoomFirstLocation.y - d.RoomSecondLocation.y == 1 && d.RoomSecond == r))
            {
                firstFloorTile = d.RoomFirst == r ? r.GetFloorTile(d.RoomFirstLocation) : r.GetFloorTile(d.RoomSecondLocation);
                secondFloorTile = d.RoomFirst == r ? d.RoomSecond.GetFloorTile(d.RoomSecondLocation) : d.RoomFirst.GetFloorTile(d.RoomFirstLocation);
                StairsLocation = d.RoomFirst == r ? d.RoomFirstLocation : d.RoomSecondLocation;
                //try each position to check for fit
                foreach (int i in positions)
                {
                    if (!positionFound)
                    {
                        switch (i)
                        {
                            case 0:
                                if (secondFloorTile.FloorSpace(1, 3, 1, 1, 9, 14) &&
                                    firstFloorTile.FloorSpace(1, 11, 1, 8, 1, 3) &&
                                    firstFloorTile.FloorSpace(1, 3, 9, 16, 1, 11))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(1, 3, 1, 1, 9, 14);
                                    firstFloorTile.AllocateFloorSpace(1, 11, 1, 8, 1, 3);
                                    firstFloorTile.AllocateFloorSpace(1, 3, 9, 16, 1, 11);
                                    
                                    position = StairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 1, position.y - 4, position.z - 6.5f);
                                    GameObject stairsOne = Instantiate(Stairs, position, Quaternion.identity);
                                    stairsOne.transform.Rotate(0, 0, 0, Space.Self);

                                    position = StairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 6.5f, position.y - 0.5f, position.z - 6.5f);
                                    GameObject platformOne = Instantiate(Platform, position, Quaternion.identity);
                                    platformOne.transform.Rotate(0, 0, 0, Space.Self);

                                    position = StairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 6.5f, position.y + 4, position.z - 1);
                                    GameObject stairsTwo = Instantiate(Stairs, position, Quaternion.identity);
                                    stairsTwo.transform.Rotate(0, 90, 0, Space.Self);

                                    stairsOne.transform.parent = r.transform;
                                    platformOne.transform.parent = r.transform;
                                    stairsTwo.transform.parent = r.transform;
                                }

                                break;
                            case 1:
                                if (secondFloorTile.FloorSpace(14, 16, 1, 1, 9, 14) &&
                                    firstFloorTile.FloorSpace(5, 16, 1, 8, 1, 3) &&
                                    firstFloorTile.FloorSpace(14, 16, 9, 16, 1, 11))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(14, 16, 1, 1, 9, 14);
                                    firstFloorTile.AllocateFloorSpace(5, 16, 1, 8, 1, 3);
                                    firstFloorTile.AllocateFloorSpace(14, 16, 9, 16, 1, 11);
                                    
                                    position = StairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + 1, position.y - 4, position.z - 6.5f);
                                    GameObject stairsOne = Instantiate(Stairs, position, Quaternion.identity);
                                    stairsOne.transform.Rotate(0, 180, 0, Space.Self);

                                    position = StairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + 6.5f, position.y - 0.5f, position.z - 6.5f);
                                    GameObject platformOne = Instantiate(Platform, position, Quaternion.identity);
                                    platformOne.transform.Rotate(0, 0, 0, Space.Self);

                                    position = StairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + 6.5f, position.y + 4, position.z - 1);
                                    GameObject stairsTwo = Instantiate(Stairs, position, Quaternion.identity);
                                    stairsTwo.transform.Rotate(0, 90, 0, Space.Self);

                                    stairsOne.transform.parent = r.transform;
                                    platformOne.transform.parent = r.transform;
                                    stairsTwo.transform.parent = r.transform;

                                    stairsOne.name = "StairsOne";
                                    platformOne.name = "PlatformOne";
                                    stairsTwo.name = "StairsTwo";
                                }

                                break;
                            case 2:
                                if (secondFloorTile.FloorSpace(1, 3, 1, 1, 3, 8) &&
                                    firstFloorTile.FloorSpace(1, 11, 1, 8, 14, 16) &&
                                    firstFloorTile.FloorSpace(1, 3, 9, 16, 6, 16))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(1, 3, 1, 1, 3, 8);
                                    firstFloorTile.AllocateFloorSpace(1, 11, 1, 8, 14, 16);
                                    firstFloorTile.AllocateFloorSpace(1, 3, 9, 16, 6, 16);
                                    
                                    position = StairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 1, position.y - 4, position.z + 6.5f);
                                    GameObject stairsOne = Instantiate(Stairs, position, Quaternion.identity);
                                    stairsOne.transform.Rotate(0, 0, 0, Space.Self);

                                    position = StairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 6.5f, position.y - 0.5f, position.z + 6.5f);
                                    GameObject platformOne = Instantiate(Platform, position, Quaternion.identity);
                                    platformOne.transform.Rotate(0, 0, 0, Space.Self);

                                    position = StairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 6.5f, position.y + 4, position.z + 1);
                                    GameObject stairsTwo = Instantiate(Stairs, position, Quaternion.identity);
                                    stairsTwo.transform.Rotate(0, 270, 0, Space.Self);

                                    stairsOne.transform.parent = r.transform;
                                    platformOne.transform.parent = r.transform;
                                    stairsTwo.transform.parent = r.transform;

                                    stairsOne.name = "StairsOne";
                                    platformOne.name = "PlatformOne";
                                    stairsTwo.name = "StairsTwo";
                                }

                                break;
                            case 3:
                                if (secondFloorTile.FloorSpace(14, 16, 1, 1, 3, 8) &&
                                    firstFloorTile.FloorSpace(5, 16, 1, 8, 14, 16) &&
                                    firstFloorTile.FloorSpace(14, 16, 9, 16, 6, 16))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(14, 16, 1, 1, 3, 8);
                                    firstFloorTile.AllocateFloorSpace(5, 16, 1, 8, 14, 16);
                                    firstFloorTile.AllocateFloorSpace(14, 16, 9, 16, 6, 16);
                                    
                                    position = StairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + 1, position.y - 4, position.z + 6.5f);
                                    GameObject stairsOne = Instantiate(Stairs, position, Quaternion.identity);
                                    stairsOne.transform.Rotate(0, 180, 0, Space.Self);

                                    position = StairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + 6.5f, position.y - 0.5f, position.z + 6.5f);
                                    GameObject platformOne = Instantiate(Platform, position, Quaternion.identity);
                                    platformOne.transform.Rotate(0, 0, 0, Space.Self);

                                    position = StairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + 6.5f, position.y + 4, position.z + 1);
                                    GameObject stairsTwo = Instantiate(Stairs, position, Quaternion.identity);
                                    stairsTwo.transform.Rotate(0, 270, 0, Space.Self);

                                    stairsOne.transform.parent = r.transform;
                                    platformOne.transform.parent = r.transform;
                                    stairsTwo.transform.parent = r.transform;

                                    stairsOne.name = "StairsOne";
                                    platformOne.name = "PlatformOne";
                                    stairsTwo.name = "StairsTwo";
                                }

                                break;
                            case 4:
                                if (secondFloorTile.FloorSpace(9, 14, 1, 1, 1, 3) &&
                                    firstFloorTile.FloorSpace(1, 3, 1, 8, 1, 11) &&
                                    firstFloorTile.FloorSpace(1, 11, 9, 16, 1, 3))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(9, 14, 1, 1, 1, 3);
                                    firstFloorTile.AllocateFloorSpace(1, 3, 1, 8, 1, 11);
                                    firstFloorTile.AllocateFloorSpace(1, 11, 9, 16, 1, 3);
                                    
                                    position = StairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 6.5f, position.y - 4, position.z - 1);
                                    GameObject stairsOne = Instantiate(Stairs, position, Quaternion.identity);
                                    stairsOne.transform.Rotate(0, 270, 0, Space.Self);

                                    position = StairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 6.5f, position.y - 0.5f, position.z - 6.5f);
                                    GameObject platformOne = Instantiate(Platform, position, Quaternion.identity);
                                    platformOne.transform.Rotate(0, 0, 0, Space.Self);

                                    position = StairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 1, position.y + 4, position.z - 6.5f);
                                    GameObject stairsTwo = Instantiate(Stairs, position, Quaternion.identity);
                                    stairsTwo.transform.Rotate(0, 180, 0, Space.Self);

                                    stairsOne.transform.parent = r.transform;
                                    platformOne.transform.parent = r.transform;
                                    stairsTwo.transform.parent = r.transform;

                                    stairsOne.name = "StairsOne";
                                    platformOne.name = "PlatformOne";
                                    stairsTwo.name = "StairsTwo";
                                }

                                break;
                            case 5:
                                if (secondFloorTile.FloorSpace(9, 14, 1, 1, 14, 16) &&
                                    firstFloorTile.FloorSpace(1, 3, 1, 8, 5, 16) &&
                                    firstFloorTile.FloorSpace(1, 11, 9, 16, 14, 16))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(9, 14, 1, 1, 14, 16);
                                    firstFloorTile.AllocateFloorSpace(1, 3, 1, 8, 5, 16);
                                    firstFloorTile.AllocateFloorSpace(1, 11, 9, 16, 14, 16);
                                    
                                    position = StairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 6.5f, position.y - 4, position.z + 1);
                                    GameObject stairsOne = Instantiate(Stairs, position, Quaternion.identity);
                                    stairsOne.transform.Rotate(0, 90, 0, Space.Self);

                                    position = StairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 6.5f, position.y - 0.5f, position.z + 6.5f);
                                    GameObject platformOne = Instantiate(Platform, position, Quaternion.identity);
                                    platformOne.transform.Rotate(0, 0, 0, Space.Self);

                                    position = StairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 1, position.y + 4, position.z + 6.5f);
                                    GameObject stairsTwo = Instantiate(Stairs, position, Quaternion.identity);
                                    stairsTwo.transform.Rotate(0, 180, 0, Space.Self);

                                    stairsOne.transform.parent = r.transform;
                                    platformOne.transform.parent = r.transform;
                                    stairsTwo.transform.parent = r.transform;

                                    stairsOne.name = "StairsOne";
                                    platformOne.name = "PlatformOne";
                                    stairsTwo.name = "StairsTwo";
                                }

                                break;
                            case 6:
                                if (secondFloorTile.FloorSpace(3, 8, 1, 1, 1, 3) &&
                                    firstFloorTile.FloorSpace(14, 16, 1, 8, 1, 11) &&
                                    firstFloorTile.FloorSpace(6, 16, 9, 16, 1, 3))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(3, 8, 1, 1, 1, 3);
                                    firstFloorTile.AllocateFloorSpace(14, 16, 1, 8, 1, 11);
                                    firstFloorTile.AllocateFloorSpace(6, 16, 9, 16, 1, 3);
                                    
                                    position = StairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + 6.5f, position.y - 4, position.z - 1);
                                    GameObject stairsOne = Instantiate(Stairs, position, Quaternion.identity);
                                    stairsOne.transform.Rotate(0, 270, 0, Space.Self);

                                    position = StairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + 6.5f, position.y - 0.5f, position.z - 6.5f);
                                    GameObject platformOne = Instantiate(Platform, position, Quaternion.identity);
                                    platformOne.transform.Rotate(0, 0, 0, Space.Self);

                                    position = StairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + 1, position.y + 4, position.z - 6.5f);
                                    GameObject stairsTwo = Instantiate(Stairs, position, Quaternion.identity);
                                    stairsTwo.transform.Rotate(0, 0, 0, Space.Self);

                                    stairsOne.transform.parent = r.transform;
                                    platformOne.transform.parent = r.transform;
                                    stairsTwo.transform.parent = r.transform;

                                    stairsOne.name = "StairsOne";
                                    platformOne.name = "PlatformOne";
                                    stairsTwo.name = "StairsTwo";
                                }

                                break;
                            default:
                                if (secondFloorTile.FloorSpace(3, 8, 1, 1, 14, 16) &&
                                    firstFloorTile.FloorSpace(14, 16, 1, 8, 5, 16) &&
                                    firstFloorTile.FloorSpace(6, 16, 9, 16, 14, 16))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(3, 8, 1, 1, 14, 16);
                                    firstFloorTile.AllocateFloorSpace(14, 16, 1, 8, 5, 16);
                                    firstFloorTile.AllocateFloorSpace(6, 16, 9, 16, 14, 16);
                                    
                                    position = StairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + 6.5f, position.y - 4, position.z + 1);
                                    GameObject stairsOne = Instantiate(Stairs, position, Quaternion.identity);
                                    stairsOne.transform.Rotate(0, 90, 0, Space.Self);

                                    position = StairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + 6.5f, position.y - 0.5f, position.z + 6.5f);
                                    GameObject platformOne = Instantiate(Platform, position, Quaternion.identity);
                                    platformOne.transform.Rotate(0, 0, 0, Space.Self);

                                    position = StairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + 1, position.y + 4, position.z + 6.5f);
                                    GameObject stairsTwo = Instantiate(Stairs, position, Quaternion.identity);
                                    stairsTwo.transform.Rotate(0, 0, 0, Space.Self);

                                    stairsOne.transform.parent = r.transform;
                                    platformOne.transform.parent = r.transform;
                                    stairsTwo.transform.parent = r.transform;

                                    stairsOne.name = "StairsOne";
                                    platformOne.name = "PlatformOne";
                                    stairsTwo.name = "StairsTwo";
                                }

                                break;
                        }
                    }
                }
            }
        }
    }

    public static void PlaceFloors(Room r)
    {

    }

    public static void PlaceDoors(Room r)
    {

    }

    public static void PlaceWalls(Room r)
    {

    }
}
