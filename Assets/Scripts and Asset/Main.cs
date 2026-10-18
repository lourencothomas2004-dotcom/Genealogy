using UnityEngine;
using SQLite;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
public class Main : MonoBehaviour
{
    public DataSet_Manager Ds;
    public Transform MainCanva;

    public GameObject Tab;
    public GameObject InfoPanel;

    public int? Id2Spawn;


    public void SpawnTab()
    {
        GameObject newTab = Instantiate(Tab, MainCanva);

        newTab.transform.localPosition = Vector3.zero;
        newTab.transform.localScale = Vector3.one;

        newTab.GetComponent<TabUI>().UpdateTabInfo(Ds.db.Find<Person>(Id2Spawn));
        newTab.GetComponent<TabUI>().InfoPanel = InfoPanel;
    }

    public void SetId2Spawn(int num)
    {
        Id2Spawn = Ds.matchingPeople[num - 1].Id;
    }
    void connections(RectTransform  parent, RectTransform  child)
    {
        Vector3 parPos = parent.position;
        Vector3 childPos = child.position;

        float zOffset = 5f;

        parPos = new Vector3(parPos.x, parPos.y, parPos.z + zOffset);
        childPos = new Vector3(childPos.x, childPos.y, childPos.z + zOffset);

        Vector3 parMidPos = new Vector3(parPos.x, (childPos.y + parPos.y)/2, parPos.z + zOffset);
        Vector3 childMidPos = new Vector3(childPos.x, (childPos.y + parPos.y)/2, childPos.z + zOffset);

        GameObject lineObj = new GameObject("Line_" + parent.name + "_to_" + child.name);
        LineRenderer  connectionLine = lineObj.AddComponent<LineRenderer>();

        connectionLine.useWorldSpace = true;
        connectionLine.sortingOrder = -1;
        connectionLine.material = new Material(Shader.Find("Sprites/Default"));
        connectionLine.startColor = Color.grey;
        connectionLine.endColor = Color.grey;
        connectionLine.startWidth = 4.0f;

        connectionLine.positionCount = 4;
        connectionLine.SetPosition(0, parPos);
        connectionLine.SetPosition(1, parMidPos);
        connectionLine.SetPosition(2, childMidPos);
        connectionLine.SetPosition(3, childPos);

    }

    public void ClearUI()
    {
        foreach (Transform child in MainCanva)
        {
            Destroy(child.gameObject);
        }
        Ds.peopleSpace = new Dictionary<Person, int>();
    }
}
