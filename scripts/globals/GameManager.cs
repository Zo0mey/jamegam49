using System.Collections.Generic;
using Godot;

namespace GameJam49Game.scripts.globals;

public partial class GameManager : Node2D
{
    public List<BlockData> blockDataList;
    
    [Signal]
    public delegate void SpawnNextBlockEventHandler();

    public void EmitSpawnNextBlock()
    {
        GD.Print("Emitting spawn next block");
        EmitSignal(SignalName.SpawnNextBlock);
    }
    
    public BlockData MapToBlockData(AnimatableBody2D animatableBody2D)
    {
        BlockData blockData = new()
        {
            Position = animatableBody2D.Position,
            Rotation = animatableBody2D.Rotation,
            Scale = animatableBody2D.Scale,
            Form = BlockForm.Square,
            Type = BlockType.Standard
        };

        return blockData;
    }
    
    public record BlockData
    {
        public Vector2 Position { get; init; }
        public float Rotation { get; init; }
        public Vector2 Scale { get; init; }
        public BlockForm Form { get; init; }
        public BlockType Type { get; init; }
    }

    public enum BlockForm
    {
        Square,
        Rectangle,
        LShape,
        TShape
    }

    public enum BlockType
    {
        Standard,
        Exploding
    }
}
