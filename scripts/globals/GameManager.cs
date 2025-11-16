using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace GameJam49Game.scripts.globals;

public partial class GameManager : Node2D
{
    [Export] public int MaxSpawnedBlocks = 1;
    public int SpawnedBlocks;
    public List<BlockData> BlockDataList;
    private GameState _currentState = GameState.Phase1;
    private bool CanSpawnBlock => SpawnedBlocks < MaxSpawnedBlocks && _currentState == GameState.Phase1;
    private CharacterBody2D _player;
    private Vector2 _flagPosition;

    [Signal]
    public delegate void SpawnNextBlockEventHandler();

    private ExplosionStartZone _explosionStartZone;


    public override void _Ready()
    {
        _explosionStartZone = GetNode<ExplosionStartZone>("/root/Main/PlayArea/ExplosionStartZone");
        if (_explosionStartZone is null)
        {
            GD.PrintErr($"Couldn't get {nameof(ExplosionStartZone)}");
        }
    }

    public override void _Process(double delta)
    {
        if (SpawnedBlocks >= MaxSpawnedBlocks && _currentState == GameState.Phase1)
        {
            _currentState = GameState.Phase2;
            StartPhase2();
        }

        if (_currentState == GameState.Phase2)
        {
            if (_explosionStartZone.CurrentExplosionHeight < _player?.GlobalPosition.Y)
            {
                GD.Print($"Gameover: {_explosionStartZone.CurrentExplosionHeight} / {_player?.GlobalPosition.Y}");
                LoseGame();
            }
        }
    }

    private Timer GetFlagTimer()
    {
        Timer flagTimer = new Timer();
        flagTimer.WaitTime = 3.0f;
        flagTimer.Autostart = false;
        AddChild(flagTimer);

        return flagTimer;
    }

    public void EmitSpawnNextBlock()
    {
        if (CanSpawnBlock)
        {
            GD.Print("Emitting spawn next block");
            EmitSignal(SignalName.SpawnNextBlock);
        }
    }

    public void WinGame()
    {
        if (_currentState == GameState.Phase2)
        {
            _currentState = GameState.Won;
            GD.Print("Win Game");
            var winGameDialog = GetNode<AcceptDialog>("/root/Main/WinGameDialog");
            winGameDialog.DialogText = $"You won the game. You tower was {_flagPosition.Y} Meters high!";
            winGameDialog.Confirmed += RestartGame;
            winGameDialog.Canceled += RestartGame;
            winGameDialog.Show();
        }
    }
    
    public void LoseGame()
    {
        if (_currentState == GameState.Phase2)
        {
            _currentState = GameState.GameOver;
            GD.Print("Lose Game");
            var winGameDialog = GetNode<AcceptDialog>("/root/Main/WinGameDialog");
            winGameDialog.Title = "You Lost!";
            winGameDialog.DialogText = "You lost the game. Try again.";
            winGameDialog.Confirmed += RestartGame;
            winGameDialog.Canceled += RestartGame;
            winGameDialog.Show();
        }
    }

    public void RestartGame()
    {
        _currentState = GameState.Phase1;
        SpawnedBlocks = 0;
        GetTree().ChangeSceneToFile("res://scenes/main.tscn");
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

    private async void StartPhase2()
    {
        Timer flagTimer = GetFlagTimer();
        flagTimer.Start();
        await ToSignal(flagTimer, "timeout");
        await SpawnFlag();
        SpawnPlayer();
        _explosionStartZone.StartExplosionsAsync();
    }

    private async Task SpawnFlag()
    {
        var flagScene = GD.Load<PackedScene>("res://scenes/flag.tscn");
        var flag = flagScene.Instantiate<Node2D>();

        var blockParent = GetNode("/root/Main/PlayArea/BlockPlacingArea");
        var highestBlock = blockParent.GetChildren()
            .Where(child => child.GetType() == typeof(SquareBlock))
            .Cast<SquareBlock>()
            .MinBy(node => node.Position.Y);

        var highestBlockSprite = highestBlock.GetNode<Sprite2D>("Sprite2D");

        Vector2 flagPosition = new Vector2(
            highestBlock.Position.X + highestBlockSprite.Texture.GetSize().X * highestBlockSprite.Scale.X / 8,
            highestBlock.Position.Y - highestBlockSprite.Texture.GetSize().Y * highestBlockSprite.Scale.Y + 5);

        _flagPosition = flagPosition;

        flag.Position = flagPosition;
        flag.Scale = new Vector2(0.5f, 0.5f);
        blockParent.AddChild(flag);
    }

    private void SpawnPlayer()
    {
        PackedScene playerScene = GD.Load<PackedScene>("res://scenes/player.tscn");
        _player = playerScene.Instantiate<CharacterBody2D>();
        Node2D mainScene = GetNode<Node2D>("/root/Main");
        _player.Scale = new Vector2(0.1f, 0.1f);
        _player.Position = new Vector2(350, 500);
        mainScene.AddChild(_player);
        var playerCamera = _player.GetNode<Camera2D>("Camera2D");
        playerCamera.MakeCurrent();
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
