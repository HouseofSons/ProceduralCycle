using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting : MonoBehaviour
{
    public enum Sky { Clear, Cloudy, Rainy, Foggy }
    public enum TimeOfDay { Morning, Noon, Twilight, Night }
    public enum Flora { Forest, Meadow, Desert, Beach, Mountain }

    public Sky sky;
    public TimeOfDay timeOfDay;
    public Flora flora;

    private static Sky skyChosen;
    private static TimeOfDay timeOfDayChosen;
    private static Flora floraChosen;

    private void Awake()
    {
        skyChosen = sky;
        timeOfDayChosen = timeOfDay;
        floraChosen = flora;
    }

    public static Sky GetSky()
    {
        return skyChosen;
    }

    public static TimeOfDay GetTimeOfDay()
    {
        return timeOfDayChosen;
    }

    public static Flora GetFlora()
    {
        return floraChosen;
    }

    public static void PopulateSettingOnWorldTerrain()
    {
        GameObject setting = new GameObject();
        setting.name = "Setting";

        if (Setting.GetFlora() == Setting.Flora.Forest)
        {
            //foreach (HexPrismChunk c in HexPrismChunk.GetChunkList())
            //{
            //    for (int i = 0; i < c.GetHexPrisms().GetLength(0); i++)
            //    {
            //        for (int j = 0; j < c.GetHexPrisms().GetLength(1); j++)
            //        {
            //            for (int k = 0; k < c.GetHexPrisms().GetLength(2); k++)
            //            {
            //                if (c.GetHexPrisms()[i, j, k] != null)
            //                {
            //                    if (c.GetHexPrisms()[i, j, k].IsTop)
            //                    {
            //                        Vector3 hexPosition = new Vector3(
            //                            HexWorldTerrain.xHexToWorldCoordinate(c.pos.x + i, c.pos.z + k),
            //                            (c.pos.y + j) * (HexWorldTerrain.GetHexScalar()) - (2 * HexWorldTerrain.GetHexScalar()),
            //                            HexWorldTerrain.zHexToWorldCoordinate(c.pos.z + k));
            //                        int rand0 = Mathf.RoundToInt(Random.Range(1, 6));
            //                        float rand1 = Random.Range(0, 0.75f);
            //                        GameObject grass = Instantiate(Resources.Load("HexGrass")) as GameObject;
            //                        grass.transform.position = new Vector3(
            //                            hexPosition.x,
            //                            hexPosition.y + (HexWorldTerrain.GetHexScalar() * 0.5f) / 2 + grass.transform.localScale.y / 2 + 0.5f - rand1,
            //                            hexPosition.z);
            //                        grass.transform.Rotate(0, rand0 * 60, 0);
            //                        grass.transform.parent = setting.transform;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
        }
    }
}