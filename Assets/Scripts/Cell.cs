using UnityEngine;
using System.Collections;

public enum Sign { Empty, X, O }
public class Cell : MonoBehaviour 
{
    public Field field;
    public GameObject O;
    public GameObject X;
    private bool _actived;
    public Sign sign = Sign.Empty; // 0 = nothing, 1 = cross, 2 = circle;
	// Use this for initialization
	void Start () {
        field = FindObjectOfType<Field>();
	}

    void OnMouseDown()
    {
        Activate();
    }

    public void Activate()
    {
        if (_actived || field.finished) return;

        _actived = true;

        if (field.CrossTurn)
            PlaceX();
        else
            PlaceO();

        field.ChangeTurn();
        field.CheckForWin();
    }

    public void PlaceX()
    {
        X.SetActive(true);
        sign = Sign.X;
        Cursor.SetCursor(field.theme.CursorO, Vector2.zero, CursorMode.Auto);

    }

    public void PlaceO()
    {
        O.SetActive(true);
        sign = Sign.O;
        Cursor.SetCursor(field.theme.CursorX, Vector2.zero, CursorMode.Auto);
    }

    public bool IsActive()
    {
        return _actived;
    }
}
