using Godot;

namespace GameJam49Game.scripts;

public partial class BlockPlacingArea : Node2D
{
    public void SpawnBlock()
    {
        PackedScene squareBlockScene = GD.Load<PackedScene>("res://scenes/square_block.tscn");
        AnimatableBody2D instantiatedBlock = squareBlockScene.Instantiate<AnimatableBody2D>();
        
        AddChild(instantiatedBlock);
    }
}