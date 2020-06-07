using UnityEngine;

public class AimArrow : MonoBehaviour
{
    public static GameObject Arrow;

    void Start()
    {
        Arrow = this.gameObject;
        Arrow.GetComponent<SpriteRenderer>().enabled = false;
    }

    public static void EnableArrowImage(bool enable)
    {
		if(enable)
        {
            Arrow.GetComponent<SpriteRenderer>().enabled = true;
		} else
        {
            Arrow.GetComponent<SpriteRenderer>().enabled = false;
		}
    }
}
