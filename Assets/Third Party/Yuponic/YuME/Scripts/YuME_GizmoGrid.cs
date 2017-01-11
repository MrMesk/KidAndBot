using UnityEngine;

// V1.01: Updated to allow for grid scale values > 1

public class YuME_GizmoGrid : MonoBehaviour
{
    [HideInInspector]
    public float tileSizeX = 1;
    [HideInInspector]
    public float tileSizeZ = 1;
    [HideInInspector]
    public int gridWidth = 40;
    [HideInInspector]
    public int gridDepth = 40;

    [HideInInspector]
    public float gridHeight
    {
        get
        {
            return _gridHeight;
        }
        set
        {
            _gridHeight = value;
        }
    }

    float _gridHeight = 0;

    [HideInInspector]
    public bool toolEnabled = true;

	public float gridOffset = 0.01f;

    float gridXZOffset = 0.5f;
    float gridYOffset = 0.5f;

    [HideInInspector]
    public Color gridColorNormal = Color.white;
    [HideInInspector]
    public Color gridColorBorder = Color.green;
    [HideInInspector]
    public Color gridColorFill = new Color(1, 0, 0, 0.5F);

    float gridWidthOffset;
    float gridDepthOffset;

    Vector3 gridColliderPosition;
    //Vector3 gridColliderSize;
    //BoxCollider gridCollider;

    void OnEnable()
    {
        gameObject.SetActive(false);
    }

    void OnDrawGizmos()
    {
        if (toolEnabled)
        {
            gridXZOffset = tileSizeX / 2;
            gridYOffset = tileSizeX / 2;
            gridWidthOffset = gridWidth * tileSizeX / 2;
            gridDepthOffset = gridDepth * tileSizeZ / 2;

            drawGridBase();
            drawMainGrid();
            drawGridBorder();

            moveGrid();
        }
    }

    public void moveGrid()
    {
        gridColliderPosition = gameObject.GetComponent<BoxCollider>().center;
        gridColliderPosition.y = gridHeight - 0.5f;
        gameObject.GetComponent<BoxCollider>().center = gridColliderPosition;
    }

    private void drawGridBorder() // fixed for scale
    {
        Gizmos.color = gridColorBorder;

        // left side

        Gizmos.DrawLine(new Vector3(0 - gridWidthOffset - gridXZOffset, gridHeight - gridYOffset - gridOffset, 0 - gridDepthOffset - gridXZOffset), 
			new Vector3(0 - gridWidthOffset - gridXZOffset, gridHeight - gridYOffset - gridOffset, (gridDepth * tileSizeZ) - gridDepthOffset - gridXZOffset));

        //bottom

		Gizmos.DrawLine(new Vector3(0 - gridWidthOffset - gridXZOffset, gridHeight - gridYOffset - gridOffset, 0 - gridDepthOffset - gridXZOffset), 
			new Vector3((gridWidth * tileSizeX) - gridWidthOffset - gridXZOffset, gridHeight - gridYOffset - gridOffset, 0 - gridDepthOffset - gridXZOffset));

        // left side

		Gizmos.DrawLine(new Vector3((gridWidth * tileSizeX) - gridWidthOffset - gridXZOffset, gridHeight - gridYOffset - gridOffset, (gridDepth * tileSizeZ) - gridDepthOffset - gridXZOffset), 
			new Vector3((gridWidth * tileSizeX) - gridWidthOffset - gridXZOffset, gridHeight - gridYOffset - gridOffset, 0 - gridDepthOffset - gridXZOffset));

        //top

		Gizmos.DrawLine(new Vector3(0 - gridWidthOffset - gridXZOffset, gridHeight - gridYOffset - gridOffset, (gridDepth * tileSizeZ) - gridDepthOffset - gridXZOffset), 
			new Vector3((gridWidth * tileSizeX) - gridWidthOffset - gridXZOffset, gridHeight - gridYOffset - gridOffset, (gridDepth * tileSizeZ) - gridDepthOffset - gridXZOffset));
    }

    private void drawGridBase() // fixed for scale
    {
        Gizmos.color = gridColorFill;
        Gizmos.DrawCube(new Vector3(0 - gridXZOffset, gridHeight - gridYOffset - gridOffset, 0 - gridXZOffset), new Vector3((gridWidth * tileSizeX), 0.01f, (gridDepth * tileSizeZ)));
    }

    private void drawMainGrid() // fixed for scale
    {
        Gizmos.color = gridColorNormal;

        if (tileSizeX != 0)
        {
            for (float i = tileSizeX; i < (gridWidth * tileSizeX); i += tileSizeX)
            {
                Gizmos.DrawLine(
					new Vector3((float)i - gridWidthOffset - gridXZOffset, gridHeight - gridYOffset - gridOffset, 0 - gridDepthOffset - gridXZOffset),
					new Vector3((float)i - gridWidthOffset - gridXZOffset, gridHeight - gridYOffset - gridOffset, (gridDepth * tileSizeZ) - gridDepthOffset - gridXZOffset));
            }
        }

        if (tileSizeZ != 0)
        {
            for (float j = tileSizeZ; j < (gridDepth * tileSizeZ) ; j += tileSizeZ)
            {
                Gizmos.DrawLine(
					new Vector3(0 - gridWidthOffset - gridXZOffset, gridHeight - gridYOffset - gridOffset, j - gridDepthOffset - gridXZOffset),
					new Vector3((gridWidth * tileSizeX) - gridWidthOffset - gridXZOffset, gridHeight - gridYOffset - gridOffset, j - gridDepthOffset - gridXZOffset));
            }
        }
    }
}
