using UnityEngine;

public class MouseTarget : MonoBehaviour
{
    [SerializeField] private LayerMask layers;

    private Camera cam;
    private Ray ray;
    private GameObject hitObject;

    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        // Moves the target according to cursor position to world space
        ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layers))
            transform.position = raycastHit.point;

        // Senses left mouse button click to place a house
        if (Input.GetMouseButtonDown(0))
        {
            PlaceHouse();
        }
    }

    // Detects which tile is the cursor pointing at
    private void OnTriggerEnter(Collider other)
    {
        hitObject = other.gameObject;
    }

    // Called when left mouse button is clicked, and returns if the tile selected is not Dirt and Desert
    private void PlaceHouse()
    {
        if (hitObject.tag != "Dirt" && hitObject.tag != "Desert") return;

        TileGenerator.SpawnHouse(int.Parse(hitObject.name));
    }
}
