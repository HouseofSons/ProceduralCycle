using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    public int Level { get; set; }
    public int Experience { get; set; }

    //Coroutine of Enemy following Path
    public Coroutine MoveCoroutine { get; private set; }
    //Coroutine of Enemy Death
    public Coroutine DeathCoroutine { get; set; }
    //Coroutine of Player gaining experience
    public Coroutine GainExperienceCoroutine { get; set; }

    void Awake()
    {
        Level = 0;
        Experience = Mathf.FloorToInt((Random.value + 0.1f) * 100);
        MoveCoroutine = null;
        DeathCoroutine = null;
        GainExperienceCoroutine = null;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (MoveCoroutine == null)
        {
            MoveCoroutine = StartCoroutine(Move(this));
        }
    }

    public static IEnumerator Move(Enemy e)
    {
        int distance = 2;

        bool testingDestination;
        Vector3 position;
        Vector3 destination;
        List<Vector3> corners = new List<Vector3>();

        do
        {
            testingDestination = true;
            position = e.transform.position;
            destination = e.transform.position;

            while (testingDestination)
            {
                int rand = Mathf.FloorToInt(Random.Range(0, 4));

                switch (rand)
                {
                    case 0:
                        destination = new Vector3(position.x + distance, position.y, position.z);
                        PositionSpriteDirection(e, false);
                        break;
                    case 1:
                        destination = new Vector3(position.x - distance, position.y, position.z);
                        PositionSpriteDirection(e, true);
                        break;
                    case 2:
                        destination = new Vector3(position.x, position.y, position.z + distance);
                        break;
                    case 3:
                        destination = new Vector3(position.x, position.y, position.z - distance);
                        break;
                }
                corners.Add(
                    new Vector3(
                        destination.x + e.transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.extents.x,
                        destination.y,
                        destination.z + e.transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.extents.z));
                corners.Add(
                    new Vector3(
                        destination.x + e.transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.extents.x,
                        destination.y,
                        destination.z - e.transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.extents.z));
                corners.Add(
                    new Vector3(
                        destination.x - e.transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.extents.x,
                        destination.y,
                        destination.z + e.transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.extents.z));
                corners.Add(
                    new Vector3(
                        destination.x - e.transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.extents.x,
                        destination.y,
                        destination.z - e.transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.extents.z));
                
                testingDestination = false;
                
                foreach (Vector3 v in corners)
                {
                    if(e.transform.parent.GetComponent<Room>().GetPartition(v) == null)
                    {
                        testingDestination = true;
                    }
                }
                corners.Clear();
            }

            while (Mathf.Abs(Vector3.Distance(e.transform.position, destination)) > 0.1f)
            {
                while (GameManager.IsPaused)
                { //for game pause
                    yield return null;
                }
                e.transform.position = Vector3.MoveTowards(e.transform.position, destination, 0.01f);
                yield return null;
            }
            e.transform.position = destination;
        } while (1 == 1);
    }

    public static void PositionSpriteDirection(Enemy e, bool left)
    {
        e.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = left;
    }

    public IEnumerator Death()
    {
        SpriteRenderer mySprite = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
        Color oldColor = mySprite.color;
        Color strobeColor = new Color(0, 0, 0, 0);
        int count = 0;

        if (MoveCoroutine != null)
        {
            StopCoroutine(MoveCoroutine);
        }

        while (count<8)
        {
            if (count % 2 == 0)
            {
                mySprite.color = strobeColor;
            }
            else
            {
                mySprite.color = oldColor;
            }

            yield return new WaitForSeconds(0.03f);
            count++;
        }
        GameObject.Destroy(this.gameObject);
    }

    public IEnumerator GainExperience()
    {
        GameObject popUp = Instantiate(Resources.Load("TextPopUp")) as GameObject;
        Vector3 position = new Vector3(this.transform.position.x + 1, 5, this.transform.position.z + 1);
        popUp.transform.position = position;
        popUp.GetComponent<TextMeshPro>().text = "+" + Experience.ToString();
        popUp.GetComponent<TextMeshPro>().fontSize += Mathf.Clamp((7 * Experience / 100), 0, 7);
        float count = -2.0f;

        while (count <= 2)
        {
            yield return null;
            popUp.transform.position = new Vector3(position.x, position.y, position.z - Mathf.Pow(count, 2) + 4);
            count += 0.1f;
        }
        while (count <= 6)
        {
            count += 0.1f;
            yield return null;
        }
        while (Mathf.Abs(Vector3.Distance(popUp.transform.position, UI.ExperienceTextWorldPos)) > 1)
        {
            popUp.transform.position = Vector3.Lerp(popUp.transform.position, UI.ExperienceTextWorldPos, 0.05f);
            yield return null;
        }
        Destroy(popUp);
    }
}