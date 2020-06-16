using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //Coroutine of Player following Path
    public Coroutine MoveCoroutine { get; private set; }

    void Awake()
    {
        MoveCoroutine = null;
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

    public static void KillEnemy(Enemy e)
    {
        GameObject.Destroy(e.gameObject);
    }

    public static void PositionSpriteDirection(Enemy e, bool left)
    {
        e.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = left;
    }

    public static void Jitter(Enemy e)
    {
        //e.transform.GetChild()
    }
}
