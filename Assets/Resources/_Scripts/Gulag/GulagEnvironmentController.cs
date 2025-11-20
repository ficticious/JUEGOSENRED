using UnityEngine;

public class GulagEnvironmentController
{
    private GameObject[] walls;
    private GameObject target;

    public GulagEnvironmentController(GameObject[] walls, GameObject target)
    {
        this.walls = walls;
        this.target = target;
    }

    public void EnableArena()
    {
        foreach (var w in walls) w.SetActive(false);
        if (target != null) target.SetActive(true);
    }

    public void DisableArena()
    {
        foreach (var w in walls) w.SetActive(true);
        if (target != null) target.SetActive(false);
    }
}
