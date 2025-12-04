using UnityEngine;

public class GulagEnvironmentController
{
    private GameObject[] walls;
    private GameObject[] targets;

    public GulagEnvironmentController(GameObject[] walls, GameObject[] targets)
    {
        this.walls = walls;
        this.targets = targets;
    }

    public void EnableArena()
    {
        foreach (var w in walls)
            w.SetActive(false);

        // Activar todos los targets
        foreach (var t in targets)
        {
            if (t != null)
                t.SetActive(true);
        }
    }

    public void DisableArena()
    {
        foreach (var w in walls)
            w.SetActive(true);

        // Desactivar todos los targets
        foreach (var t in targets)
        {
            if (t != null)
                t.SetActive(false);
        }
    }
}
