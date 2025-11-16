using System.Collections.Generic;
using System.Linq;
using Godot;

namespace GameJam49Game.scripts.globals;

public partial class GameManager : Node2D
{
    [Export] public int MaxSpawnedBlocks = 1;
    public int SpawnedBlocks;
    public List<BlockData> BlockDataList;
    private GameState _currentState = GameState.Phase1;
    private bool canSpawnBlock => SpawnedBlocks < MaxSpawnedBlocks && _currentState == GameState.Phase1;

    [Signal]
    public delegate void SpawnNextBlockEventHandler();

    public void EmitSpawnNextBlock()
    {

        if (canSpawnBlock)
        {
            GD.Print("Emitting spawn next block");
            EmitSignal(SignalName.SpawnNextBlock);
        }
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

    public override void _Process(double delta)
    {
        if (SpawnedBlocks >= MaxSpawnedBlocks && _currentState == GameState.Phase1)
        {
            _currentState = GameState.Phase2;
            PackedScene playerScene = GD.Load<PackedScene>("res://scenes/player.tscn");
            CharacterBody2D player = playerScene.Instantiate<CharacterBody2D>();
            player.Scale = new Vector2(0.25f, 0.25f);
            player.Position = new Vector2(450, 300);
            AddChild(player);

            var flagScene = GD.Load<PackedScene>("res://scenes/flag.tscn");
            var flag = flagScene.Instantiate<Node2D>();

            var blockParent = GetNode("/root/Main/PlayArea/BlockPlacingArea");
            var highestBlock = blockParent.GetChildren()
                .Where(child => child.Name == "SquareBlock")
                .Cast<Node2D>()
                .MaxBy(node => node.Position.Y);
            flag.Position = highestBlock.Position;
            AddChild(flag);

            // TODO: Start explosions from bottom
        }

    }

    private void GetHighestBlock()
    {

    }

    // Start Game -> automatisch
    // 1 Phase: Place blocks
    // Track Blocks, ca. 20 blocks
    // 2 Phase: Climb tower
    // Win condition: Flag
    // Lose condition: Explosions from bottom

    // -----------------------------------------------------------------------------------------------------------

    public record BlockData
    {
        public Vector2 Position { get; init; }
        public float Rotation { get; init; }
        public Vector2 Scale { get; init; }
        public BlockForm Form { get; init; }
        public BlockType Type { get; init; }
    }

    public enum GameState
    {
        Phase1,
        Phase2,
        GameOver,
        Won,
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
