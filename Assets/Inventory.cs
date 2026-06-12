using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int capacity;
    public Transform slotsGroup;
    private Transform[] slots;
    private GameObject[] cubes = new GameObject[20];

    public void Awake()
    {
        capacity = 20;
        initializeSlots();
    }

    public void initializeSlots()
    {
        slots = new Transform[capacity];
        for (int i = 0; i < capacity; i++)
        {
            slots[i] = slotsGroup.GetChild(i);
        }
    }

    public GameObject[] getCubes()
    {
        for (int i = 0; i < capacity; i++)
        {
            cubes[i] = slots[i].childCount > 0 ? slots[i].GetChild(0).gameObject : null;
        }
        return cubes;
    }

    public int getCubesCount()
    {
        int count = 0;
        for (int i = 0; i < capacity; i++)
        {
            if (cubes[i] != null) count++;
        }
        return count;
    }

    public Transform ClaimNextFreeSlot(out int claimedIndex)
    {
        for (int i = 0; i < capacity; i++)
        {
            if (slots[i].childCount == 0)
            {
                claimedIndex = i;
                return slots[i];
            }
        }
        claimedIndex = -1;
        return null;
    }

    public void setCube(GameObject givenCube, int claimedIndex)
    {
        cubes[claimedIndex] = givenCube;
        givenCube.transform.SetParent(slots[claimedIndex]);
    }

    public void clearSlot(int index)
    {
        cubes[index] = null;
    }
}