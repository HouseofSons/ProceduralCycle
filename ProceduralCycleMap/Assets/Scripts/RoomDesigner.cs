using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDesigner : MonoBehaviour
{
    public static RoomDesigner instance;
    public GameObject stairs;
    public GameObject spiralStairs;
    public GameObject platform;

    public static GameObject Stairs { get { return instance.stairs; } }
    public static GameObject SpiralStairs { get { return instance.spiralStairs; } }
    public static GameObject Platform { get { return instance.platform; } }

    private void Awake()
    {
        instance = this;
    }

    public static void PlaceStairs(Room r)
    {
        List<int> positions = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        Seed.Shuffle(positions);
        FloorTile firstFloorTile;
        FloorTile secondFloorTile;
        bool positionFound;
        Vector3 position;

        foreach (Door d in r.Doors)
        {
            //Debug.Log("Room: " + r.Order + " Door1: " + d.RoomFirst.Order + " @ " + d.RoomFirstLocation + " Door2: " + d.RoomSecond.Order + " @ " + d.RoomSecondLocation);
            positionFound = false;

            Vector3Int stairsLocation;

            //identify above neighbor
            if ((d.RoomSecondLocation.y - d.RoomFirstLocation.y == 1 && d.RoomFirst == r) ||
                (d.RoomFirstLocation.y - d.RoomSecondLocation.y == 1 && d.RoomSecond == r))
            {
                if (d.RoomFirst != d.RoomSecond)
                {
                    firstFloorTile = d.RoomFirst == r ? r.GetFloorTile(d.RoomFirstLocation) : r.GetFloorTile(d.RoomSecondLocation);
                    secondFloorTile = d.RoomFirst == r ? d.RoomSecond.GetFloorTile(d.RoomSecondLocation) : d.RoomFirst.GetFloorTile(d.RoomFirstLocation);
                    stairsLocation = d.RoomFirst == r ? d.RoomFirstLocation : d.RoomSecondLocation;
                }
                else
                {
                    firstFloorTile = d.RoomSecondLocation.y > d.RoomFirstLocation.y ? r.GetFloorTile(d.RoomFirstLocation) : r.GetFloorTile(d.RoomSecondLocation);
                    secondFloorTile = d.RoomSecondLocation.y > d.RoomFirstLocation.y ? d.RoomSecond.GetFloorTile(d.RoomSecondLocation) : d.RoomFirst.GetFloorTile(d.RoomFirstLocation);
                    stairsLocation = d.RoomSecondLocation.y > d.RoomFirstLocation.y ? d.RoomFirstLocation : d.RoomSecondLocation;
                }
                //try each position to check for fit
                foreach (int i in positions)
                {
                    if (!positionFound)
                    {
                        switch (i)
                        {
                            case 0: //Corner Stairs
                                if (secondFloorTile.FloorSpace(1, 3, 1, 1, 9, 14) &&
                                    firstFloorTile.FloorSpace(1, 11, 1, 8, 1, 3) &&
                                    firstFloorTile.FloorSpace(1, 3, 9, 16, 1, 11))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(1, 3, 1, 1, 9, 14);
                                    firstFloorTile.AllocateFloorSpace(1, 11, 1, 8, 1, 3);
                                    firstFloorTile.AllocateFloorSpace(1, 3, 9, 16, 1, 11);
                                    
                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 1, position.y - 4, position.z - 6.5f);
                                    GameObject stairsOne = Instantiate(Stairs, position, Quaternion.identity);
                                    stairsOne.transform.Rotate(0, 0, 0, Space.Self);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 6.5f, position.y - 0.5f, position.z - 6.5f);
                                    GameObject platformOne = Instantiate(Platform, position, Quaternion.identity);
                                    platformOne.transform.Rotate(0, 0, 0, Space.Self);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 6.5f, position.y + 4, position.z - 1);
                                    GameObject stairsTwo = Instantiate(Stairs, position, Quaternion.identity);
                                    stairsTwo.transform.Rotate(0, 90, 0, Space.Self);

                                    stairsOne.transform.parent = r.transform;
                                    platformOne.transform.parent = r.transform;
                                    stairsTwo.transform.parent = r.transform;
                                }

                                break;
                            case 1: //Corner Stairs
                                if (secondFloorTile.FloorSpace(14, 16, 1, 1, 9, 14) &&
                                    firstFloorTile.FloorSpace(5, 16, 1, 8, 1, 3) &&
                                    firstFloorTile.FloorSpace(14, 16, 9, 16, 1, 11))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(14, 16, 1, 1, 9, 14);
                                    firstFloorTile.AllocateFloorSpace(5, 16, 1, 8, 1, 3);
                                    firstFloorTile.AllocateFloorSpace(14, 16, 9, 16, 1, 11);
                                    
                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + 1, position.y - 4, position.z - 6.5f);
                                    GameObject stairsOne = Instantiate(Stairs, position, Quaternion.identity);
                                    stairsOne.transform.Rotate(0, 180, 0, Space.Self);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + 6.5f, position.y - 0.5f, position.z - 6.5f);
                                    GameObject platformOne = Instantiate(Platform, position, Quaternion.identity);
                                    platformOne.transform.Rotate(0, 0, 0, Space.Self);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
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
                            case 2: //Corner Stairs
                                if (secondFloorTile.FloorSpace(1, 3, 1, 1, 3, 8) &&
                                    firstFloorTile.FloorSpace(1, 11, 1, 8, 14, 16) &&
                                    firstFloorTile.FloorSpace(1, 3, 9, 16, 6, 16))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(1, 3, 1, 1, 3, 8);
                                    firstFloorTile.AllocateFloorSpace(1, 11, 1, 8, 14, 16);
                                    firstFloorTile.AllocateFloorSpace(1, 3, 9, 16, 6, 16);
                                    
                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 1, position.y - 4, position.z + 6.5f);
                                    GameObject stairsOne = Instantiate(Stairs, position, Quaternion.identity);
                                    stairsOne.transform.Rotate(0, 0, 0, Space.Self);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 6.5f, position.y - 0.5f, position.z + 6.5f);
                                    GameObject platformOne = Instantiate(Platform, position, Quaternion.identity);
                                    platformOne.transform.Rotate(0, 0, 0, Space.Self);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
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
                            case 3: //Corner Stairs
                                if (secondFloorTile.FloorSpace(14, 16, 1, 1, 3, 8) &&
                                    firstFloorTile.FloorSpace(5, 16, 1, 8, 14, 16) &&
                                    firstFloorTile.FloorSpace(14, 16, 9, 16, 6, 16))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(14, 16, 1, 1, 3, 8);
                                    firstFloorTile.AllocateFloorSpace(5, 16, 1, 8, 14, 16);
                                    firstFloorTile.AllocateFloorSpace(14, 16, 9, 16, 6, 16);
                                    
                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + 1, position.y - 4, position.z + 6.5f);
                                    GameObject stairsOne = Instantiate(Stairs, position, Quaternion.identity);
                                    stairsOne.transform.Rotate(0, 180, 0, Space.Self);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + 6.5f, position.y - 0.5f, position.z + 6.5f);
                                    GameObject platformOne = Instantiate(Platform, position, Quaternion.identity);
                                    platformOne.transform.Rotate(0, 0, 0, Space.Self);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
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
                            case 4: //Corner Stairs
                                if (secondFloorTile.FloorSpace(9, 14, 1, 1, 1, 3) &&
                                    firstFloorTile.FloorSpace(1, 3, 1, 8, 1, 11) &&
                                    firstFloorTile.FloorSpace(1, 11, 9, 16, 1, 3))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(9, 14, 1, 1, 1, 3);
                                    firstFloorTile.AllocateFloorSpace(1, 3, 1, 8, 1, 11);
                                    firstFloorTile.AllocateFloorSpace(1, 11, 9, 16, 1, 3);
                                    
                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 6.5f, position.y - 4, position.z - 1);
                                    GameObject stairsOne = Instantiate(Stairs, position, Quaternion.identity);
                                    stairsOne.transform.Rotate(0, 270, 0, Space.Self);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 6.5f, position.y - 0.5f, position.z - 6.5f);
                                    GameObject platformOne = Instantiate(Platform, position, Quaternion.identity);
                                    platformOne.transform.Rotate(0, 0, 0, Space.Self);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
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
                            case 5: //Corner Stairs
                                if (secondFloorTile.FloorSpace(9, 14, 1, 1, 14, 16) &&
                                    firstFloorTile.FloorSpace(1, 3, 1, 8, 5, 16) &&
                                    firstFloorTile.FloorSpace(1, 11, 9, 16, 14, 16))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(9, 14, 1, 1, 14, 16);
                                    firstFloorTile.AllocateFloorSpace(1, 3, 1, 8, 5, 16);
                                    firstFloorTile.AllocateFloorSpace(1, 11, 9, 16, 14, 16);
                                    
                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 6.5f, position.y - 4, position.z + 1);
                                    GameObject stairsOne = Instantiate(Stairs, position, Quaternion.identity);
                                    stairsOne.transform.Rotate(0, 90, 0, Space.Self);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 6.5f, position.y - 0.5f, position.z + 6.5f);
                                    GameObject platformOne = Instantiate(Platform, position, Quaternion.identity);
                                    platformOne.transform.Rotate(0, 0, 0, Space.Self);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
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
                            case 6: //Corner Stairs
                                if (secondFloorTile.FloorSpace(3, 8, 1, 1, 1, 3) &&
                                    firstFloorTile.FloorSpace(14, 16, 1, 8, 1, 11) &&
                                    firstFloorTile.FloorSpace(6, 16, 9, 16, 1, 3))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(3, 8, 1, 1, 1, 3);
                                    firstFloorTile.AllocateFloorSpace(14, 16, 1, 8, 1, 11);
                                    firstFloorTile.AllocateFloorSpace(6, 16, 9, 16, 1, 3);
                                    
                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + 6.5f, position.y - 4, position.z - 1);
                                    GameObject stairsOne = Instantiate(Stairs, position, Quaternion.identity);
                                    stairsOne.transform.Rotate(0, 270, 0, Space.Self);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + 6.5f, position.y - 0.5f, position.z - 6.5f);
                                    GameObject platformOne = Instantiate(Platform, position, Quaternion.identity);
                                    platformOne.transform.Rotate(0, 0, 0, Space.Self);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
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
                            case 7: //Corner Stairs
                                if (secondFloorTile.FloorSpace(3, 8, 1, 1, 14, 16) &&
                                    firstFloorTile.FloorSpace(14, 16, 1, 8, 5, 16) &&
                                    firstFloorTile.FloorSpace(6, 16, 9, 16, 14, 16))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(3, 8, 1, 1, 14, 16);
                                    firstFloorTile.AllocateFloorSpace(14, 16, 1, 8, 5, 16);
                                    firstFloorTile.AllocateFloorSpace(6, 16, 9, 16, 14, 16);
                                    
                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + 6.5f, position.y - 4, position.z + 1);
                                    GameObject stairsOne = Instantiate(Stairs, position, Quaternion.identity);
                                    stairsOne.transform.Rotate(0, 90, 0, Space.Self);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + 6.5f, position.y - 0.5f, position.z + 6.5f);
                                    GameObject platformOne = Instantiate(Platform, position, Quaternion.identity);
                                    platformOne.transform.Rotate(0, 0, 0, Space.Self);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
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
                            case 8: //Spiral Stairs
                                if (secondFloorTile.FloorSpace(8, 13, 1, 1, 4, 6) &&
                                    firstFloorTile.FloorSpace(11, 13, 1, 8, 4, 9) &&
                                    firstFloorTile.FloorSpace(8, 10, 9, 16, 4, 9))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(8, 13, 1, 1, 4, 6);
                                    firstFloorTile.AllocateFloorSpace(11, 13, 1, 8, 4, 9);
                                    firstFloorTile.AllocateFloorSpace(8, 10, 9, 16, 4, 9);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + 1.5f, position.y - 4, position.z - 1.5f);
                                    GameObject stairsOne = Instantiate(SpiralStairs, position, Quaternion.identity);
                                    stairsOne.transform.Rotate(0, 0, 0, Space.Self);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + 1.5f, position.y + 4, position.z - 1.5f);
                                    GameObject stairsTwo = Instantiate(SpiralStairs, position, Quaternion.identity);
                                    stairsTwo.transform.Rotate(0, 180, 0, Space.Self);

                                    stairsOne.transform.parent = r.transform;
                                    stairsTwo.transform.parent = r.transform;

                                    stairsOne.name = "SpiralStairsOne";
                                    stairsTwo.name = "SpiralStairsTwo";
                                }

                                break;
                            case 9: //Spiral Stairs
                                if (secondFloorTile.FloorSpace(8, 13, 1, 1, 7, 9) &&
                                    firstFloorTile.FloorSpace(8, 10, 9, 16, 4, 9) &&
                                    firstFloorTile.FloorSpace(11, 13, 1, 8, 4, 9))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(8, 13, 1, 1, 7, 9);
                                    firstFloorTile.AllocateFloorSpace(8, 10, 9, 16, 4, 9);
                                    firstFloorTile.AllocateFloorSpace(11, 13, 1, 8, 4, 9);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + 1.5f, position.y - 4, position.z - 1.5f);
                                    GameObject stairsOne = Instantiate(SpiralStairs, position, Quaternion.identity);
                                    stairsOne.transform.Rotate(0, 180, 0, Space.Self);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + 1.5f, position.y + 4, position.z - 1.5f);
                                    GameObject stairsTwo = Instantiate(SpiralStairs, position, Quaternion.identity);
                                    stairsTwo.transform.Rotate(0, 0, 0, Space.Self);

                                    stairsOne.transform.parent = r.transform;
                                    stairsTwo.transform.parent = r.transform;

                                    stairsOne.name = "SpiralStairsOne";
                                    stairsTwo.name = "SpiralStairsTwo";
                                }

                                break;

                            case 10: //Spiral Stairs
                                if (secondFloorTile.FloorSpace(4, 9, 1, 1, 4, 6) &&
                                    firstFloorTile.FloorSpace(7, 9, 1, 8, 4, 9) &&
                                    firstFloorTile.FloorSpace(4, 6, 9, 16, 4, 9))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(4, 9, 1, 1, 4, 6);
                                    firstFloorTile.AllocateFloorSpace(7, 9, 1, 8, 4, 9);
                                    firstFloorTile.AllocateFloorSpace(4, 6, 9, 16, 4, 9);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 1.5f, position.y - 4, position.z - 1.5f);
                                    GameObject stairsOne = Instantiate(SpiralStairs, position, Quaternion.identity);
                                    stairsOne.transform.Rotate(0, 0, 0, Space.Self);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 1.5f, position.y + 4, position.z - 1.5f);
                                    GameObject stairsTwo = Instantiate(SpiralStairs, position, Quaternion.identity);
                                    stairsTwo.transform.Rotate(0, 180, 0, Space.Self);

                                    stairsOne.transform.parent = r.transform;
                                    stairsTwo.transform.parent = r.transform;

                                    stairsOne.name = "SpiralStairsOne";
                                    stairsTwo.name = "SpiralStairsTwo";
                                }

                                break;
                            case 11: //Spiral Stairs
                                if (secondFloorTile.FloorSpace(4, 9, 1, 1, 7, 9) &&
                                    firstFloorTile.FloorSpace(4, 6, 1, 8, 4, 9) &&
                                    firstFloorTile.FloorSpace(7, 9, 9, 16, 4, 9))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(4, 9, 1, 1, 7, 9);
                                    firstFloorTile.AllocateFloorSpace(4, 6, 1, 8, 4, 9);
                                    firstFloorTile.AllocateFloorSpace(7, 9, 9, 16, 4, 9);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 1.5f, position.y - 4, position.z - 1.5f);
                                    GameObject stairsOne = Instantiate(SpiralStairs, position, Quaternion.identity);
                                    stairsOne.transform.Rotate(0, 180, 0, Space.Self);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 1.5f, position.y + 4, position.z - 1.5f);
                                    GameObject stairsTwo = Instantiate(SpiralStairs, position, Quaternion.identity);
                                    stairsTwo.transform.Rotate(0, 0, 0, Space.Self);

                                    stairsOne.transform.parent = r.transform;
                                    stairsTwo.transform.parent = r.transform;

                                    stairsOne.name = "SpiralStairsOne";
                                    stairsTwo.name = "SpiralStairsTwo";
                                }

                                break;

                            case 12: //Spiral Stairs
                                if (secondFloorTile.FloorSpace(4, 9, 1, 1, 8, 10) &&
                                    firstFloorTile.FloorSpace(7, 9, 1, 8, 8, 13) &&
                                    firstFloorTile.FloorSpace(4, 6, 9, 16, 8, 13))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(4, 9, 1, 1, 8, 10);
                                    firstFloorTile.AllocateFloorSpace(7, 9, 1, 8, 8, 13);
                                    firstFloorTile.AllocateFloorSpace(4, 6, 9, 16, 8, 13);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 1.5f, position.y - 4, position.z + 1.5f);
                                    GameObject stairsOne = Instantiate(SpiralStairs, position, Quaternion.identity);
                                    stairsOne.transform.Rotate(0, 0, 0, Space.Self);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 1.5f, position.y + 4, position.z + 1.5f);
                                    GameObject stairsTwo = Instantiate(SpiralStairs, position, Quaternion.identity);
                                    stairsTwo.transform.Rotate(0, 180, 0, Space.Self);

                                    stairsOne.transform.parent = r.transform;
                                    stairsTwo.transform.parent = r.transform;

                                    stairsOne.name = "SpiralStairsOne";
                                    stairsTwo.name = "SpiralStairsTwo";
                                }

                                break;
                            case 13: //Spiral Stairs
                                if (secondFloorTile.FloorSpace(4, 9, 1, 1, 11, 13) &&
                                    firstFloorTile.FloorSpace(4, 6, 1, 8, 8, 13) &&
                                    firstFloorTile.FloorSpace(7, 9, 9, 16, 8, 13))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(4, 9, 1, 1, 11, 13);
                                    firstFloorTile.AllocateFloorSpace(4, 6, 1, 8, 8, 13);
                                    firstFloorTile.AllocateFloorSpace(7, 9, 9, 16, 8, 13);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 1.5f, position.y - 4, position.z + 1.5f);
                                    GameObject stairsOne = Instantiate(SpiralStairs, position, Quaternion.identity);
                                    stairsOne.transform.Rotate(0, 180, 0, Space.Self);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x - 1.5f, position.y + 4, position.z + 1.5f);
                                    GameObject stairsTwo = Instantiate(SpiralStairs, position, Quaternion.identity);
                                    stairsTwo.transform.Rotate(0, 0, 0, Space.Self);

                                    stairsOne.transform.parent = r.transform;
                                    stairsTwo.transform.parent = r.transform;

                                    stairsOne.name = "SpiralStairsOne";
                                    stairsTwo.name = "SpiralStairsTwo";
                                }

                                break;

                            case 14: //Spiral Stairs
                                if (secondFloorTile.FloorSpace(8, 13, 1, 1, 8, 10) &&
                                    firstFloorTile.FloorSpace(11, 13, 1, 8, 8, 13) &&
                                    firstFloorTile.FloorSpace(8, 10, 9, 16, 8, 13))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(8, 13, 1, 1, 8, 10);
                                    firstFloorTile.AllocateFloorSpace(11, 13, 1, 8, 8, 13);
                                    firstFloorTile.AllocateFloorSpace(8, 10, 9, 16, 8, 13);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + 1.5f, position.y - 4, position.z + 1.5f);
                                    GameObject stairsOne = Instantiate(SpiralStairs, position, Quaternion.identity);
                                    stairsOne.transform.Rotate(0, 0, 0, Space.Self);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + 1.5f, position.y + 4, position.z + 1.5f);
                                    GameObject stairsTwo = Instantiate(SpiralStairs, position, Quaternion.identity);
                                    stairsTwo.transform.Rotate(0, 180, 0, Space.Self);

                                    stairsOne.transform.parent = r.transform;
                                    stairsTwo.transform.parent = r.transform;

                                    stairsOne.name = "SpiralStairsOne";
                                    stairsTwo.name = "SpiralStairsTwo";
                                }

                                break;
                            default: //Spiral Stairs
                                if (secondFloorTile.FloorSpace(8, 13, 1, 1, 11, 13) &&
                                    firstFloorTile.FloorSpace(8, 10, 1, 8, 8, 13) &&
                                    firstFloorTile.FloorSpace(11, 13, 9, 16, 8, 13))
                                {
                                    positionFound = true;
                                    secondFloorTile.AllocateFloorSpace(8, 13, 1, 1, 11, 13);
                                    firstFloorTile.AllocateFloorSpace(8, 10, 1, 8, 8, 13);
                                    firstFloorTile.AllocateFloorSpace(11, 13, 9, 16, 8, 13);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + 1.5f, position.y - 4, position.z + 1.5f);
                                    GameObject stairsOne = Instantiate(SpiralStairs, position, Quaternion.identity);
                                    stairsOne.transform.Rotate(0, 180, 0, Space.Self);

                                    position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                                    position = new Vector3(position.x + 1.5f, position.y + 4, position.z + 1.5f);
                                    GameObject stairsTwo = Instantiate(SpiralStairs, position, Quaternion.identity);
                                    stairsTwo.transform.Rotate(0, 0, 0, Space.Self);

                                    stairsOne.transform.parent = r.transform;
                                    stairsTwo.transform.parent = r.transform;

                                    stairsOne.name = "SpiralStairsOne";
                                    stairsTwo.name = "SpiralStairsTwo";
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
