using UnityEngine.InputSystem;
using UnityEngine;
using DG.Tweening;

public class GameManagerDemo : MonoBehaviour
{
    public Inventory inventory;
    public Car activeCar1;
    public Car activeCar2;
    public Material BlueColor;
    public Material PurpleColor;
    public Material RedColor;
    public Material YellowColor;
    public Transform activeSlot1;
    public Transform activeSlot2;
    public int numOfCars;
    public InGameUI inGameUI;

    private bool isAnimating = false;

    void Start()
    {
        inventory.initializeSlots();
        activeCar1.initializeSlots();
        activeCar2.initializeSlots();

        activeCar1.getInPosition(activeSlot1.position, activeCar1.left);

        activeCar2.getInPosition(activeSlot2.position, activeCar2.left);

        activeCar1.onDriveOff = () => { activeCar1 = null; numOfCars--; checkWinCondition(); };
        activeCar2.onDriveOff = () => { activeCar2 = null; numOfCars--; checkWinCondition(); };
    }

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && !isAnimating)
        {
            HandleClick(Mouse.current.position.ReadValue());
        }

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame && !isAnimating)
        {
            HandleClick(Touchscreen.current.primaryTouch.position.ReadValue());
        }
    }

    private void HandleClick(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Car car = hit.collider.GetComponent<Car>();
            if (car == null) return;

            if (car == activeCar1 || car == activeCar2)
            {
                if (car.isInPosition())
                    HandleCarClick(car);
                return;
            }

            if (car.isReady())
            {
                if (car.left && activeCar2 == null)
                {
                    activeCar2 = car;
                    car.onDriveOff = () => { activeCar2 = null; numOfCars--; checkWinCondition(); };
                    car.onCarReady = () => HandleCarClick(activeCar2);
                    car.getInPosition(activeSlot2.position, car.left);
                }
                else if (!car.left && activeCar1 == null)
                {
                    activeCar1 = car;
                    car.onDriveOff = () => { activeCar1 = null; numOfCars--; checkWinCondition(); };
                    car.onCarReady = () => HandleCarClick(activeCar1);
                    car.getInPosition(activeSlot1.position, car.left);
                }
            }
        }
    }

    private void HandleCarClick(Car car)
    {
        //car.resetFixedCubes();
        GameObject[] cubes = car.getCubes();
        if (cubes == null || cubes.Length == 0) return;

        isAnimating = true;
        int completed = 0;
        int total = cubes.Length;

        for (int i = 0; i < total; i++)
        {
            int index = i;

            if (cubes[index] == null)
            {
                completed++;
                if (completed == total) OnAllCubesInInventory(car, cubes);
                continue;
            }

            Transform targetSlot = inventory.ClaimNextFreeSlot(out int claimedIndex);
            if (targetSlot == null)
            {
                completed++;
                if (completed == total) OnAllCubesInInventory(car, cubes);
                continue;
            }

            GameObject cube = cubes[index];
            cubes[index] = null;
            cube.transform.SetParent(targetSlot);
            int savedIndex = claimedIndex;

            DOVirtual.DelayedCall(index * 0.1f, () =>
            {
                cube.transform.DOMove(targetSlot.position, 0.4f)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() =>
                    {
                        inventory.setCube(cube, savedIndex);
                        completed++;
                        if (completed == total) OnAllCubesInInventory(car, cubes);
                    });
            });
        }
    }

    private void OnAllCubesInInventory(Car car, GameObject[] cubes)
    {
        DistributeCubes(() =>
        {
            isAnimating = false;
            checkWinCondition();
        });
    }

    private void DistributeCubes(System.Action onComplete = null)
    {
        GameObject[] cubes = inventory.getCubes();
        int animIndex = 0;
        int total = 0;
        int completed = 0;

        for (int i = 0; i < cubes.Length; i++)
        {
            if (cubes[i] == null) continue;

            Car targetCar = null;
            if (CanAcceptCube(activeCar1, cubes[i])) targetCar = activeCar1;
            else if (CanAcceptCube(activeCar2, cubes[i])) targetCar = activeCar2;

            if (targetCar == null) continue;

            total++;
            int delay = animIndex++;
            GameObject cube = cubes[i];
            cubes[i] = null;
            inventory.clearSlot(i);

            DOVirtual.DelayedCall(delay * 0.1f, () =>
            {
                targetCar.setCube(cube);
                completed++;
                if (completed == total) onComplete?.Invoke();
            });
        }

        if (total == 0) onComplete?.Invoke();
    }

    private bool CanAcceptCube(Car car, GameObject cube)
    {
        if (car == null || cube == null) return false;
        Renderer cubeR = cube.GetComponent<Renderer>();
        if (cubeR == null) return false;
        Renderer carR = car.GetRenderer();
        if (carR == null) return false;
        return cubeR.sharedMaterial == carR.sharedMaterials[0];
    }

    public void checkWinCondition()
    {
        if (numOfCars == 0 && inventory.getCubesCount() == 0)
        {
            inGameUI.ShowWinPanel();
            return;
        }

        if (inventory.getCubesCount() == inventory.capacity && activeCar1 != null && activeCar2 != null)
        {
            inGameUI.ShowLosePanel();
            return;
        }

        if (activeCar1 != null && activeCar2 != null)
        {
            bool car1Empty = activeCar1.getCubesCount() == 0;
            bool car2Empty = activeCar2.getCubesCount() == 0;
            if (car1Empty && car2Empty)
                inGameUI.ShowLosePanel();
        }
    }
}