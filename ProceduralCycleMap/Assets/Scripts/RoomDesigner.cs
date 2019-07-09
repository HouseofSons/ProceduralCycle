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

    public static void PlaceStairsToRoom(Room r)
    {
        FloorTile firstFloorTile;
        FloorTile secondFloorTile;
        Vector3Int stairsLocation;

        foreach (Door d in r.Doors)
        {
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
                PlaceStairs(firstFloorTile, secondFloorTile, stairsLocation);
            }
        }
    }

    public static void PlaceStairsInPath(Room r)
    {
        Vector3Int pathStepPrev;
        Vector3Int pathStepCurr = Vector3Int.zero;

        if (r.Path.Count > 0) {
            foreach (Vector3Int p in r.Path)
            {
                pathStepPrev = pathStepCurr;
                pathStepCurr = p;

                if (pathStepPrev != Vector3Int.zero)
                {
                    if (pathStepPrev.y < pathStepCurr.y)
                    {
                        PlaceStairs(r.GetFloorTile(pathStepPrev), r.GetFloorTile(pathStepCurr), pathStepPrev);
                    }
                    if (pathStepPrev.y > pathStepCurr.y)
                    {
                        PlaceStairs(r.GetFloorTile(pathStepCurr), r.GetFloorTile(pathStepPrev), pathStepCurr);
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

    public static void PlaceStairs(FloorTile bottom, FloorTile top, Vector3Int stairsLocation)
    {
        List<int> positions = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        Seed.Shuffle(positions);
        bool positionFound = false;
        Vector3 position;

        //Debug.Log("Room: " + r.Order + " Door1: " + d.RoomFirst.Order + " @ " + d.RoomFirstLocation + " Door2: " + d.RoomSecond.Order + " @ " + d.RoomSecondLocation);

        //try each position to check for fit
        foreach (int i in positions)
        {
            if (!positionFound)
            {
                switch (i)
                {
                    case 0: //Corner Stairs
                        if (top.FloorSpace(1, 3, 1, 1, 9, 14) &&
                            bottom.FloorSpace(1, 11, 1, 8, 1, 3) &&
                            bottom.FloorSpace(1, 3, 9, 16, 1, 11))
                        {
                            positionFound = true;
                            top.AllocateFloorHole(1, 3, 1, 1, 9, 11);
                            top.AllocateFloorOccupied(1, 3, 1, 1, 12, 14);
                            bottom.AllocateFloorOccupied(1, 11, 1, 8, 1, 3);
                            bottom.AllocateFloorOccupied(1, 3, 9, 16, 1, 11);

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

                            stairsOne.transform.parent = bottom.Room.transform;
                            platformOne.transform.parent = bottom.Room.transform;
                            stairsTwo.transform.parent = bottom.Room.transform;

                            stairsOne.name = "StairsOne";
                            platformOne.name = "PlatformOne";
                            stairsTwo.name = "StairsTwo";
                        }

                        break;
                    case 1: //Corner Stairs
                        if (top.FloorSpace(14, 16, 1, 1, 9, 14) &&
                            bottom.FloorSpace(5, 16, 1, 8, 1, 3) &&
                            bottom.FloorSpace(14, 16, 9, 16, 1, 11))
                        {
                            positionFound = true;
                            top.AllocateFloorHole(14, 16, 1, 1, 9, 11);
                            top.AllocateFloorOccupied(14, 16, 1, 1, 12, 14);
                            bottom.AllocateFloorOccupied(5, 16, 1, 8, 1, 3);
                            bottom.AllocateFloorOccupied(14, 16, 9, 16, 1, 11);

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

                            stairsOne.transform.parent = bottom.Room.transform;
                            platformOne.transform.parent = bottom.Room.transform;
                            stairsTwo.transform.parent = bottom.Room.transform;

                            stairsOne.name = "StairsOne";
                            platformOne.name = "PlatformOne";
                            stairsTwo.name = "StairsTwo";
                        }

                        break;
                    case 2: //Corner Stairs
                        if (top.FloorSpace(1, 3, 1, 1, 3, 8) &&
                            bottom.FloorSpace(1, 11, 1, 8, 14, 16) &&
                            bottom.FloorSpace(1, 3, 9, 16, 6, 16))
                        {
                            positionFound = true;
                            top.AllocateFloorHole(1, 3, 1, 1, 6, 8);
                            top.AllocateFloorOccupied(1, 3, 1, 1, 3, 5);
                            bottom.AllocateFloorOccupied(1, 11, 1, 8, 14, 16);
                            bottom.AllocateFloorOccupied(1, 3, 9, 16, 6, 16);

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

                            stairsOne.transform.parent = bottom.Room.transform;
                            platformOne.transform.parent = bottom.Room.transform;
                            stairsTwo.transform.parent = bottom.Room.transform;

                            stairsOne.name = "StairsOne";
                            platformOne.name = "PlatformOne";
                            stairsTwo.name = "StairsTwo";
                        }

                        break;
                    case 3: //Corner Stairs
                        if (top.FloorSpace(14, 16, 1, 1, 3, 8) &&
                            bottom.FloorSpace(5, 16, 1, 8, 14, 16) &&
                            bottom.FloorSpace(14, 16, 9, 16, 6, 16))
                        {
                            positionFound = true;
                            top.AllocateFloorHole(14, 16, 1, 1, 6, 8);
                            top.AllocateFloorOccupied(14, 16, 1, 1, 3, 5);
                            bottom.AllocateFloorOccupied(5, 16, 1, 8, 14, 16);
                            bottom.AllocateFloorOccupied(14, 16, 9, 16, 6, 16);

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

                            stairsOne.transform.parent = bottom.Room.transform;
                            platformOne.transform.parent = bottom.Room.transform;
                            stairsTwo.transform.parent = bottom.Room.transform;

                            stairsOne.name = "StairsOne";
                            platformOne.name = "PlatformOne";
                            stairsTwo.name = "StairsTwo";
                        }

                        break;
                    case 4: //Corner Stairs
                        if (top.FloorSpace(9, 14, 1, 1, 1, 3) &&
                            bottom.FloorSpace(1, 3, 1, 8, 1, 11) &&
                            bottom.FloorSpace(1, 11, 9, 16, 1, 3))
                        {
                            positionFound = true;
                            top.AllocateFloorHole(9, 11, 1, 1, 1, 3);
                            top.AllocateFloorOccupied(12, 14, 1, 1, 1, 3);
                            bottom.AllocateFloorOccupied(1, 3, 1, 8, 1, 11);
                            bottom.AllocateFloorOccupied(1, 11, 9, 16, 1, 3);

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

                            stairsOne.transform.parent = bottom.Room.transform;
                            platformOne.transform.parent = bottom.Room.transform;
                            stairsTwo.transform.parent = bottom.Room.transform;

                            stairsOne.name = "StairsOne";
                            platformOne.name = "PlatformOne";
                            stairsTwo.name = "StairsTwo";
                        }

                        break;
                    case 5: //Corner Stairs
                        if (top.FloorSpace(9, 14, 1, 1, 14, 16) &&
                            bottom.FloorSpace(1, 3, 1, 8, 5, 16) &&
                            bottom.FloorSpace(1, 11, 9, 16, 14, 16))
                        {
                            positionFound = true;
                            top.AllocateFloorHole(9, 11, 1, 1, 14, 16);
                            top.AllocateFloorOccupied(12, 14, 1, 1, 14, 16);
                            bottom.AllocateFloorOccupied(1, 3, 1, 8, 5, 16);
                            bottom.AllocateFloorOccupied(1, 11, 9, 16, 14, 16);

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

                            stairsOne.transform.parent = bottom.Room.transform;
                            platformOne.transform.parent = bottom.Room.transform;
                            stairsTwo.transform.parent = bottom.Room.transform;

                            stairsOne.name = "StairsOne";
                            platformOne.name = "PlatformOne";
                            stairsTwo.name = "StairsTwo";
                        }

                        break;
                    case 6: //Corner Stairs
                        if (top.FloorSpace(3, 8, 1, 1, 1, 3) &&
                            bottom.FloorSpace(14, 16, 1, 8, 1, 11) &&
                            bottom.FloorSpace(6, 16, 9, 16, 1, 3))
                        {
                            positionFound = true;
                            top.AllocateFloorHole(6, 8, 1, 1, 1, 3);
                            top.AllocateFloorOccupied(3, 5, 1, 1, 1, 3);
                            bottom.AllocateFloorOccupied(14, 16, 1, 8, 1, 11);
                            bottom.AllocateFloorOccupied(6, 16, 9, 16, 1, 3);

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

                            stairsOne.transform.parent = bottom.Room.transform;
                            platformOne.transform.parent = bottom.Room.transform;
                            stairsTwo.transform.parent = bottom.Room.transform;

                            stairsOne.name = "StairsOne";
                            platformOne.name = "PlatformOne";
                            stairsTwo.name = "StairsTwo";
                        }

                        break;
                    case 7: //Corner Stairs
                        if (top.FloorSpace(3, 8, 1, 1, 14, 16) &&
                            bottom.FloorSpace(14, 16, 1, 8, 5, 16) &&
                            bottom.FloorSpace(6, 16, 9, 16, 14, 16))
                        {
                            positionFound = true;
                            top.AllocateFloorHole(6, 8, 1, 1, 14, 16);
                            top.AllocateFloorOccupied(3, 5, 1, 1, 14, 16);
                            bottom.AllocateFloorOccupied(14, 16, 1, 8, 5, 16);
                            bottom.AllocateFloorOccupied(6, 16, 9, 16, 14, 16);

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

                            stairsOne.transform.parent = bottom.Room.transform;
                            platformOne.transform.parent = bottom.Room.transform;
                            stairsTwo.transform.parent = bottom.Room.transform;

                            stairsOne.name = "StairsOne";
                            platformOne.name = "PlatformOne";
                            stairsTwo.name = "StairsTwo";
                        }

                        break;
                    case 8: //Spiral Stairs
                        if (top.FloorSpace(8, 13, 1, 1, 4, 6) &&
                            bottom.FloorSpace(11, 13, 1, 8, 4, 9) &&
                            bottom.FloorSpace(8, 10, 9, 16, 4, 9))
                        {
                            positionFound = true;
                            top.AllocateFloorHole(11, 13, 1, 1, 4, 6);
                            top.AllocateFloorOccupied(8, 10, 1, 1, 4, 6);
                            bottom.AllocateFloorOccupied(11, 13, 1, 8, 4, 9);
                            bottom.AllocateFloorOccupied(8, 10, 9, 16, 4, 9);

                            position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                            position = new Vector3(position.x + 1.5f, position.y - 4, position.z - 1.5f);
                            GameObject stairsOne = Instantiate(SpiralStairs, position, Quaternion.identity);
                            stairsOne.transform.Rotate(0, 0, 0, Space.Self);

                            position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                            position = new Vector3(position.x + 1.5f, position.y + 4, position.z - 1.5f);
                            GameObject stairsTwo = Instantiate(SpiralStairs, position, Quaternion.identity);
                            stairsTwo.transform.Rotate(0, 180, 0, Space.Self);

                            stairsOne.transform.parent = bottom.Room.transform;
                            stairsTwo.transform.parent = bottom.Room.transform;

                            stairsOne.name = "SpiralStairsOne";
                            stairsTwo.name = "SpiralStairsTwo";
                        }

                        break;
                    case 9: //Spiral Stairs
                        if (top.FloorSpace(8, 13, 1, 1, 7, 9) &&
                            bottom.FloorSpace(8, 10, 9, 16, 4, 9) &&
                            bottom.FloorSpace(11, 13, 1, 8, 4, 9))
                        {
                            positionFound = true;
                            top.AllocateFloorHole(8, 10, 1, 1, 7, 9);
                            top.AllocateFloorOccupied(11, 13, 1, 1, 7, 9);
                            bottom.AllocateFloorOccupied(8, 10, 9, 16, 4, 9);
                            bottom.AllocateFloorOccupied(11, 13, 1, 8, 4, 9);

                            position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                            position = new Vector3(position.x + 1.5f, position.y - 4, position.z - 1.5f);
                            GameObject stairsOne = Instantiate(SpiralStairs, position, Quaternion.identity);
                            stairsOne.transform.Rotate(0, 180, 0, Space.Self);

                            position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                            position = new Vector3(position.x + 1.5f, position.y + 4, position.z - 1.5f);
                            GameObject stairsTwo = Instantiate(SpiralStairs, position, Quaternion.identity);
                            stairsTwo.transform.Rotate(0, 0, 0, Space.Self);

                            stairsOne.transform.parent = bottom.Room.transform;
                            stairsTwo.transform.parent = bottom.Room.transform;

                            stairsOne.name = "SpiralStairsOne";
                            stairsTwo.name = "SpiralStairsTwo";
                        }

                        break;

                    case 10: //Spiral Stairs
                        if (top.FloorSpace(4, 9, 1, 1, 4, 6) &&
                            bottom.FloorSpace(7, 9, 1, 8, 4, 9) &&
                            bottom.FloorSpace(4, 6, 9, 16, 4, 9))
                        {
                            positionFound = true;
                            top.AllocateFloorHole(7, 9, 1, 1, 4, 6);
                            top.AllocateFloorOccupied(4, 6, 1, 1, 4, 6);
                            bottom.AllocateFloorOccupied(7, 9, 1, 8, 4, 9);
                            bottom.AllocateFloorOccupied(4, 6, 9, 16, 4, 9);

                            position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                            position = new Vector3(position.x - 1.5f, position.y - 4, position.z - 1.5f);
                            GameObject stairsOne = Instantiate(SpiralStairs, position, Quaternion.identity);
                            stairsOne.transform.Rotate(0, 0, 0, Space.Self);

                            position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                            position = new Vector3(position.x - 1.5f, position.y + 4, position.z - 1.5f);
                            GameObject stairsTwo = Instantiate(SpiralStairs, position, Quaternion.identity);
                            stairsTwo.transform.Rotate(0, 180, 0, Space.Self);

                            stairsOne.transform.parent = bottom.Room.transform;
                            stairsTwo.transform.parent = bottom.Room.transform;

                            stairsOne.name = "SpiralStairsOne";
                            stairsTwo.name = "SpiralStairsTwo";
                        }

                        break;
                    case 11: //Spiral Stairs
                        if (top.FloorSpace(4, 9, 1, 1, 7, 9) &&
                            bottom.FloorSpace(4, 6, 1, 8, 4, 9) &&
                            bottom.FloorSpace(7, 9, 9, 16, 4, 9))
                        {
                            positionFound = true;
                            top.AllocateFloorHole(4, 6, 1, 1, 7, 9);
                            top.AllocateFloorOccupied(7, 9, 1, 1, 7, 9);
                            bottom.AllocateFloorOccupied(4, 6, 1, 8, 4, 9);
                            bottom.AllocateFloorOccupied(7, 9, 9, 16, 4, 9);

                            position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                            position = new Vector3(position.x - 1.5f, position.y - 4, position.z - 1.5f);
                            GameObject stairsOne = Instantiate(SpiralStairs, position, Quaternion.identity);
                            stairsOne.transform.Rotate(0, 180, 0, Space.Self);

                            position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                            position = new Vector3(position.x - 1.5f, position.y + 4, position.z - 1.5f);
                            GameObject stairsTwo = Instantiate(SpiralStairs, position, Quaternion.identity);
                            stairsTwo.transform.Rotate(0, 0, 0, Space.Self);

                            stairsOne.transform.parent = bottom.Room.transform;
                            stairsTwo.transform.parent = bottom.Room.transform;

                            stairsOne.name = "SpiralStairsOne";
                            stairsTwo.name = "SpiralStairsTwo";
                        }

                        break;

                    case 12: //Spiral Stairs
                        if (top.FloorSpace(4, 9, 1, 1, 8, 10) &&
                            bottom.FloorSpace(7, 9, 1, 8, 8, 13) &&
                            bottom.FloorSpace(4, 6, 9, 16, 8, 13))
                        {
                            positionFound = true;
                            top.AllocateFloorHole(7, 9, 1, 1, 8, 10);
                            top.AllocateFloorOccupied(4, 6, 1, 1, 8, 10);
                            bottom.AllocateFloorOccupied(7, 9, 1, 8, 8, 13);
                            bottom.AllocateFloorOccupied(4, 6, 9, 16, 8, 13);

                            position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                            position = new Vector3(position.x - 1.5f, position.y - 4, position.z + 1.5f);
                            GameObject stairsOne = Instantiate(SpiralStairs, position, Quaternion.identity);
                            stairsOne.transform.Rotate(0, 0, 0, Space.Self);

                            position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                            position = new Vector3(position.x - 1.5f, position.y + 4, position.z + 1.5f);
                            GameObject stairsTwo = Instantiate(SpiralStairs, position, Quaternion.identity);
                            stairsTwo.transform.Rotate(0, 180, 0, Space.Self);

                            stairsOne.transform.parent = bottom.Room.transform;
                            stairsTwo.transform.parent = bottom.Room.transform;

                            stairsOne.name = "SpiralStairsOne";
                            stairsTwo.name = "SpiralStairsTwo";
                        }

                        break;
                    case 13: //Spiral Stairs
                        if (top.FloorSpace(4, 9, 1, 1, 11, 13) &&
                            bottom.FloorSpace(4, 6, 1, 8, 8, 13) &&
                            bottom.FloorSpace(7, 9, 9, 16, 8, 13))
                        {
                            positionFound = true;
                            top.AllocateFloorHole(4, 6, 1, 1, 11, 13);
                            top.AllocateFloorOccupied(7, 9, 1, 1, 11, 13);
                            bottom.AllocateFloorOccupied(4, 6, 1, 8, 8, 13);
                            bottom.AllocateFloorOccupied(7, 9, 9, 16, 8, 13);

                            position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                            position = new Vector3(position.x - 1.5f, position.y - 4, position.z + 1.5f);
                            GameObject stairsOne = Instantiate(SpiralStairs, position, Quaternion.identity);
                            stairsOne.transform.Rotate(0, 180, 0, Space.Self);

                            position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                            position = new Vector3(position.x - 1.5f, position.y + 4, position.z + 1.5f);
                            GameObject stairsTwo = Instantiate(SpiralStairs, position, Quaternion.identity);
                            stairsTwo.transform.Rotate(0, 0, 0, Space.Self);

                            stairsOne.transform.parent = bottom.Room.transform;
                            stairsTwo.transform.parent = bottom.Room.transform;

                            stairsOne.name = "SpiralStairsOne";
                            stairsTwo.name = "SpiralStairsTwo";
                        }

                        break;

                    case 14: //Spiral Stairs
                        if (top.FloorSpace(8, 13, 1, 1, 8, 10) &&
                            bottom.FloorSpace(11, 13, 1, 8, 8, 13) &&
                            bottom.FloorSpace(8, 10, 9, 16, 8, 13))
                        {
                            positionFound = true;
                            top.AllocateFloorHole(11, 13, 1, 1, 8, 10);
                            top.AllocateFloorOccupied(8, 10, 1, 1, 8, 10);
                            bottom.AllocateFloorOccupied(11, 13, 1, 8, 8, 13);
                            bottom.AllocateFloorOccupied(8, 10, 9, 16, 8, 13);

                            position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                            position = new Vector3(position.x + 1.5f, position.y - 4, position.z + 1.5f);
                            GameObject stairsOne = Instantiate(SpiralStairs, position, Quaternion.identity);
                            stairsOne.transform.Rotate(0, 0, 0, Space.Self);

                            position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                            position = new Vector3(position.x + 1.5f, position.y + 4, position.z + 1.5f);
                            GameObject stairsTwo = Instantiate(SpiralStairs, position, Quaternion.identity);
                            stairsTwo.transform.Rotate(0, 180, 0, Space.Self);

                            stairsOne.transform.parent = bottom.Room.transform;
                            stairsTwo.transform.parent = bottom.Room.transform;

                            stairsOne.name = "SpiralStairsOne";
                            stairsTwo.name = "SpiralStairsTwo";
                        }

                        break;
                    default: //Spiral Stairs
                        if (top.FloorSpace(8, 13, 1, 1, 11, 13) &&
                            bottom.FloorSpace(8, 10, 1, 8, 8, 13) &&
                            bottom.FloorSpace(11, 13, 9, 16, 8, 13))
                        {
                            positionFound = true;
                            top.AllocateFloorHole(8, 10, 1, 1, 11, 13);
                            top.AllocateFloorOccupied(11, 13, 1, 1, 11, 13);
                            bottom.AllocateFloorOccupied(8, 10, 1, 8, 8, 13);
                            bottom.AllocateFloorOccupied(11, 13, 9, 16, 8, 13);

                            position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                            position = new Vector3(position.x + 1.5f, position.y - 4, position.z + 1.5f);
                            GameObject stairsOne = Instantiate(SpiralStairs, position, Quaternion.identity);
                            stairsOne.transform.Rotate(0, 180, 0, Space.Self);

                            position = stairsLocation * ProceduralMapController.ROOM_SCALE;
                            position = new Vector3(position.x + 1.5f, position.y + 4, position.z + 1.5f);
                            GameObject stairsTwo = Instantiate(SpiralStairs, position, Quaternion.identity);
                            stairsTwo.transform.Rotate(0, 0, 0, Space.Self);

                            stairsOne.transform.parent = bottom.Room.transform;
                            stairsTwo.transform.parent = bottom.Room.transform;

                            stairsOne.name = "SpiralStairsOne";
                            stairsTwo.name = "SpiralStairsTwo";
                        }

                        break;
                }
            }
        }
    }
}
