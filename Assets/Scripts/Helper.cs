using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{
    public static Helper instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FindObjectwithTag(string _tag, GameObject temp, List<GameObject> actors)
    {
        actors.Clear();
        Transform parent = temp.transform;
        GetChildObject(parent, _tag, actors);
    }

    public void GetChildObject(Transform parent, string _tag, List<GameObject> actors)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);

            if (child.tag == _tag)
            {
                actors.Add(child.gameObject);
            }

            if (child.childCount > 0)
            {
                GetChildObject(child, _tag, actors);
            }
        }
    }
}
