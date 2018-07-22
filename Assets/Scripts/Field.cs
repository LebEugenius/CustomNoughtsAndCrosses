using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;
public enum Players { Player, EasyAI }
public class Field : MonoBehaviour
{
    [Range(1,3)]
    public int newSize;

    private int _oldSize;

    public Cell[,] grid;
    public Cell cellPrefab;
    [SerializeField] int cellsToWin = 3;
    int cellSize = 2;
    public bool CrossTurn = true;

    [SerializeField] UI UI;
    public bool finished;

    public Theme theme;

    public Players PlayerOne;
    public Players PlayerTwo;

    void Start()
    {
        StartNewGame();
    }

    public void Finish(string winner)
    {
        Cursor.SetCursor(theme.CursorDefault, Vector2.zero, CursorMode.Auto);
        finished = true;
        DeleteField();
        UI.Finish(winner);
    }

    public void StartNewGame()
    {
        DeleteField();
        _oldSize = newSize*3;
        CreateField();
        finished = false;
        Cursor.SetCursor(theme.CursorX, Vector2.zero, CursorMode.Auto);
        CrossTurn = true;
    }

    public void CreateField()
    {
        pvp = UI.isPvp;
        grid = new Cell[_oldSize, _oldSize];

        cellsToWin = Mathf.CeilToInt(_oldSize / 3f) + 2;
        Debug.Log(Mathf.CeilToInt(_oldSize / 3f));

        //Scale Camera
        Camera.main.transform.position = new Vector3(_oldSize - 1, _oldSize - 1, -10);
        Camera.main.orthographicSize = _oldSize + 1;

        //Create Cells and set default sprites
        for (var i = 0; i < _oldSize; i++)
            for (var j = 0; j < _oldSize; j++)
            {
                grid[i, j] = Instantiate(cellPrefab, new Vector2(i * cellSize, j * cellSize), Quaternion.identity);

                grid[i, j].transform.localScale = new Vector3(cellSize, cellSize);
                grid[i, j].GetComponentInChildren<SpriteRenderer>().sprite = theme.Default;
            }

        //Left Down 
        var cornerCell = grid[0, 0].GetComponentInChildren<SpriteRenderer>();
        cornerCell.sprite = theme.Corner;
        cornerCell.transform.rotation = Quaternion.Euler(0, 0, 0);

        //Left Top
        cornerCell = grid[0, _oldSize-1].GetComponentInChildren<SpriteRenderer>();
        cornerCell.sprite = theme.Corner;
        cornerCell.transform.rotation = Quaternion.Euler(0, 0, -90);

        //Right Top
        cornerCell = grid[_oldSize-1, _oldSize-1].GetComponentInChildren<SpriteRenderer>();
        cornerCell.sprite = theme.Corner;
        cornerCell.transform.rotation = Quaternion.Euler(0, 0, -180);

        //Right Down
        cornerCell = grid[_oldSize-1, 0].GetComponentInChildren<SpriteRenderer>();
        cornerCell.sprite = theme.Corner;
        cornerCell.transform.rotation = Quaternion.Euler(0, 0, -270);

        //Top and down sides' cells
        for(var i = 1; i < _oldSize - 1; i++)
        {
            grid[i, _oldSize - 1].GetComponentInChildren<SpriteRenderer>().sprite = theme.TopDownSide;
            grid[i, 0].GetComponentInChildren<SpriteRenderer>().sprite = theme.TopDownSide;
        }

        //Right and Left sides' cells
        for (var i = 1; i < _oldSize - 1; i++)
        {
            grid[_oldSize - 1, i].GetComponentInChildren<SpriteRenderer>().sprite = theme.RightLeftSide;
            grid[0, i].GetComponentInChildren<SpriteRenderer>().sprite = theme.RightLeftSide;
        }
    }

    public void DeleteField()
    {
        for (var i = 0; i < _oldSize; i++)
            for (var j = 0; j < _oldSize; j++)
            {
                if(grid[i, j])
                    Destroy(grid[i, j].gameObject);
            }
    }

    public void ChangeTurn()
    {
        CrossTurn = !CrossTurn;
        if (CrossTurn && PlayerOne != Players.Player)
        {
            ComputerTurn();
        }
        if(!CrossTurn && PlayerTwo != Players.Player)
        {
            ComputerTurn();
        }
    }

    #region WinCheck

    public void CheckForWin()
    {
        var winSign = CheckForHorizontalWin();

        if(winSign == Sign.Empty)
            winSign = CheckForVerticalWin();

        if(winSign == Sign.Empty)
            winSign = CheckForDiagonalWinFromRight();

        if(winSign == Sign.Empty)
            winSign = CheckForDiagonalWinFromLeft();       

        switch (winSign)
        {
            case Sign.X:
                Finish("X");
                return;
            case Sign.O:
                Finish("O");
                return;
        }

        if (!CheckForTie()) return;
        Finish("Tie");
    }

    Sign CheckForHorizontalWin()
    {
        for (int i = 0; i < _oldSize; i++) // Кол-во столбцов для проверки
        {     
            for (int j = 0; j < _oldSize - cellsToWin + 1; j++) // Кол-во проверок в столбце
            {
                int winCells = 0;
                for (int c = 0; c < cellsToWin - 1; c++) // Кол-во клеток для победы
                {
                    if (grid[j, i].sign != 0 && grid[j + c + 1, i].sign != 0) // Если 2 соседние проверяемые клетки не пусты, то
                    {
                        if (grid[j, i].sign == grid[j + c + 1, i].sign) // Прверяем равны ли их знаки
                            winCells++; // Если да, то 2 клетки становятся победными
                        else
                            winCells = 0; // А иначе становятся обычными
                    }
                    if (cellsToWin - 1 == winCells) //Если кол-во победных клеток равно необходимому кол-ву клеток
                    {
                        return grid[j, i].sign; //Возращаем знак победившего
                    }
                }
            }
        }
        return Sign.Empty;
    }

    Sign CheckForVerticalWin()
    {
        for (int i = 0; i < _oldSize; i++)
        {
            for (int j = 0; j < _oldSize - cellsToWin + 1; j++)
            {
                int winCells = 0;
                for (int c = 0; c < cellsToWin - 1; c++)
                {
                    if (grid[i, j].sign != 0 && grid[i, j + c + 1].sign != 0)
                    {
                        if (grid[i, j].sign == grid[i, j + c + 1].sign)
                            winCells++;
                        else
                            winCells = 0;
                    }
                    if (cellsToWin - 1 == winCells)
                    {
                        return grid[i, j].sign;
                    }
                }
            }
        }
        return Sign.Empty;
    }

    Sign CheckForDiagonalWinFromLeft()
    {
        for (int i = 0; i < _oldSize - cellsToWin + 1; i++)
        {
            for (int j = 0; j < _oldSize - cellsToWin + 1; j++)
            {
                int winCells = 0;
                for (int c = 0; c < cellsToWin - 1; c++)
                {
                    if (grid[j, i].sign != 0 && grid[j + c + 1, i + c + 1].sign != 0)
                    {
                        if (grid[j, i].sign == grid[j + c + 1, i + c + 1].sign)
                            winCells++;
                        else
                            winCells = 0;
                    }

                    if (cellsToWin - 1 == winCells) //Если кол-во победных клеток равно необходимому кол-ву клеток
                    {
                        return grid[j, i].sign; //Возращаем знак победившего
                    }
                }
            }
        }
        return Sign.Empty;
    }

    Sign CheckForDiagonalWinFromRight()
    {
        for (int i = _oldSize - 1; i >= _oldSize - (_oldSize - cellsToWin + 1); i--)
        {
            for (int j = 0; j < _oldSize - cellsToWin + 1; j++)
            {
                int winCells = 0;
                for (int c = 0; c < cellsToWin - 1; c++)
                {
                    if (grid[j, i].sign != 0 && grid[j + c + 1, i - c - 1].sign != 0)
                    {
                        if (grid[j, i].sign == grid[j + c + 1, i - c - 1].sign)
                            winCells++;
                        else
                            winCells = 0;
                    }
                    if (cellsToWin - 1 == winCells) //Если кол-во победных клеток равно необходимому кол-ву клеток
                    {
                        return grid[j, i].sign; //Возращаем знак победившего
                    }
                }
            }
        }
        return Sign.Empty;
    }

    bool CheckForTie()
    {
        return grid.Cast<Cell>().Count(cell => cell.sign != 0) ==  _oldSize * _oldSize;
    }
    #endregion

    #region AI
    private Cell priorityCell;

    public void ComputerTurn()
    {
        if(CheckForTie()) return;

        var currentCell = grid[0, 0];

        do
        {
            var rx = Random.Range(0, _oldSize);
            var ry = Random.Range(0, _oldSize);
            currentCell = grid[rx, ry];
        } while (currentCell.IsActive());

        currentCell.Activate();
    }

    #endregion
}
