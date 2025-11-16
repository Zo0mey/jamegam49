using Godot;
using GameJam49Game.scripts.globals;

public partial class Flag : Node2D
{
	private GameManager _gameManager = null!;

	public override void _Ready()
	{
		_gameManager = GetNode<GameManager>("/root/GameManager");
		var area2D = GetNode<Area2D>("Area2D");
		area2D.BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		GD.Print("Collision With Flag");
		if (body is PlayerMovementController)
		{
			_gameManager.WinGame();
		}
	}
}
