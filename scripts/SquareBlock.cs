using Godot;
using GameJam49Game.scripts.globals;

public partial class SquareBlock : CharacterBody2D
{
	[Export]
	public int Speed { get; set; } = 400;

	[Export]
	public int Gravitation { get; set; } = 2000;

	public bool CanBeMoved { get; set; } = true;

	public GameManager.BlockForm BlockForm = GameManager.BlockForm.Square;
	public GameManager.BlockType BlockType { get; set; }

	public void ApplyInput(double delta)
	{
		Vector2 inputDirection = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

		if (CanBeMoved && (inputDirection == Vector2.Left || inputDirection == Vector2.Right || inputDirection == Vector2.Down))
		{
			Velocity = inputDirection * Speed;
			// add gravitation
			Velocity += Vector2.Down * Gravitation * (float)delta;
		}
		else if (CanBeMoved)
		{
			Velocity = Vector2.Down * Gravitation * (float)delta;
		}
		else
		{
			Velocity = Vector2.Down * Gravitation * 10 * (float)delta;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		ApplyInput(delta);
		MoveAndSlide();
	}
}
