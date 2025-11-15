using GameJam49Game.scripts.globals;
using Godot;

public partial class MovementLine : Area2D
{
    private GameManager _gameManager = null!;
    
    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>("/root/GameManager");
        Area2D area2D = GetNode<Area2D>(".");
        area2D.BodyEntered += OnBodyEntered;
    }
    
    private void OnBodyEntered(Node2D body)
    {
        if (body is SquareBlock squareBlock)
        {
            squareBlock.CanBeMoved = false;
            GD.Print("Cant move square block");
            _gameManager.EmitSpawnNextBlock();
        }
    }

}
