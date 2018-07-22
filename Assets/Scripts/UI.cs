using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI : MonoBehaviour {

    Field field;
    public Text winner;
    public Text size;
    public Text pvp;
	
    void Start()
    {
        field = FindObjectOfType<Field>();
    }

	public void Finish (string winner) {
        if (winner == "Tie")
            this.winner.text = winner;
        else
            this.winner.text = winner + " Win";
    }

    public void Restart()
    {
        field.StartNewGame();
        this.winner.text = "";
    }

    public void ChangeSize()
    {
        if (++field.newSize > 3)
            field.newSize = 1;
        size.text = "Size of field " + field.newSize*3 + "x" + field.newSize*3;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ChangeMode()
    {
        if(field.PlayerOne == Players.Player && field.PlayerTwo == Players.Player)
        {
            field.PlayerOne = Players.Player;
            field.PlayerTwo = Players.EasyAI;
            pvp.text = "Player vs EasyAI";
        }
        else if (field.PlayerOne == Players.Player && field.PlayerTwo == Players.EasyAI)
        {
            field.PlayerOne = Players.EasyAI;
            field.PlayerTwo = Players.EasyAI;
            pvp.text = "EasyAI vs EasyAI";
        }
        else
        {
            field.PlayerOne = Players.Player;
            field.PlayerTwo = Players.Player;
            pvp.text = "Player vs Player";
        }
    }
}
