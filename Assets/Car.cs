using UnityEngine;
using DG.Tweening;
using System.Collections;

public class Car : MonoBehaviour
{
    private int capacity;
    private GameObject[] cubes;
    private Renderer bodyRenderer;
    public GameObject Cube;
    public Transform slotsGroup;
    private Transform[] slots;
    private int fixedCubes;
    public bool left;
    public Car[] carsInQueue;
    public bool ready;
    private bool inPosition;
    public System.Action onCarReady;
    public System.Action onDriveOff;

    public void Awake()
    {
        capacity = slotsGroup.childCount;
        bodyRenderer = this.GetComponent<Renderer>();
        fixedCubes = 0;
        initializeSlots();
        initializeExistingCubes();
        ready = false;
    }

    public void initializeSlots()
    {
        slots = new Transform[capacity];
        for (int i = 0; i < capacity; i++)
        {
            slots[i] = slotsGroup.GetChild(i);
        }
    }

    public void initializeExistingCubes()
    {
        cubes = new GameObject[capacity];
        for (int i = 0; i < capacity; i++)
        {
            if (slots[i].childCount > 0)
                cubes[i] = slots[i].GetChild(0).gameObject;
        }
    }

    public GameObject[] getCubes()
    {
        /*for (int i = 0; i < capacity; i++)
        {
            cubes[i] = slots[i].childCount > 0 ? slots[i].GetChild(0).gameObject : null;
        }*/
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

    private int pendingCubes = 0;

    public bool setCube(GameObject givenCube)
    {
        if (fixedCubes + pendingCubes >= capacity) return false;

        for (int i = 0; i < capacity; i++)
        {
            if (cubes[i] == null)
            {
                int index = i;
                cubes[index] = givenCube;
                pendingCubes++;
                cubes[index].transform.DOMove(slots[index].position, 0.3f)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        cubes[index].transform.SetParent(slots[index]);
                        Destroy(cubes[index]);
                        cubes[index] = null;
                        pendingCubes--;
                        fixedCubes++;
                        if (fixedCubes == capacity)
                        {
                            makeCarsReady();
                            onDriveOff?.Invoke();
                            DriveOff();
                        }
                    });
                return true;
            }
        }
        return false;
    }

    public int getCapacity()
    {
        return capacity;
    }

    public Renderer GetRenderer()
    {
        return bodyRenderer;
    }

    public void DriveOff(System.Action onComplete = null)
    {
        Sequence seq = DOTween.Sequence();

        Quaternion turnedRotation = transform.localRotation * Quaternion.Euler(0, 0, left ? -90f : 90f);
        seq.Append(transform.DOLocalRotateQuaternion(turnedRotation, 0.5f)
            .SetEase(Ease.InOutSine));

        Vector3 offscreenPos = transform.position + new Vector3(-50f, 0, 0);
        seq.Append(transform.DOMove(offscreenPos, 1.5f)
            .SetEase(Ease.InCubic));

        seq.OnComplete(() =>
        {
            onComplete?.Invoke();
            Destroy(gameObject);
        });
    }

    public void showCubes()
    {
        for (int i = 0; i < capacity; i++)
        {
            if (cubes[i] != null)
                cubes[i].SetActive(true);
        }
    }

    public void hideCubes()
    {
        for (int i = 0; i < capacity; i++)
        {
            if (cubes[i] != null)
                cubes[i].SetActive(false);
        }
    }

    public void getInPosition(Vector3 targetPosition, bool left)
    {
        inPosition = false;
        Quaternion targetRotation = Quaternion.Euler(270, left ? 0 : 180, 90);
        transform.DOMove(targetPosition, 1.5f).SetEase(Ease.InOutSine);
        transform.DORotateQuaternion(targetRotation, 1.5f).SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                inPosition = true;
                onCarReady?.Invoke();
            });
    }

    public void makeCarsReady()
    {
        if (carsInQueue == null) return;
        foreach (Car car in carsInQueue)
        {
            car.ready = true;
        }
    }

    public void setInPosition()
    {
        inPosition = true;
    }

    public bool isReady()
    {
        return ready;
    }

    public bool isInPosition()
    {
        return inPosition;
    }

    public void resetFixedCubes()
    {
        fixedCubes = 0;
        pendingCubes = 0;
    }
}