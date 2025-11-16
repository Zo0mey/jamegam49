using GameJam49Game.scripts.globals;
using Godot;

namespace GameJam49Game.scripts;

public partial class BlockPlacingArea : Node2D
{
    private RandomNumberGenerator _rng;
    private GameManager _gameManager = null!;
    
    public override void _Ready()
    {
        _rng = new RandomNumberGenerator();
        _gameManager = GetNode<GameManager>("/root/GameManager");
        _gameManager.SpawnNextBlock += StartSpawnTimer;

        StartSpawnTimer();
    }

    private void StartSpawnTimer()
    {
        GD.Print("StartSpawnTimer");
        Timer blockSpawnTimer = new Timer();
        AddChild(blockSpawnTimer);
        blockSpawnTimer.WaitTime = 3.0f;
        blockSpawnTimer.OneShot = true;
        blockSpawnTimer.Connect("timeout", Callable.From(OnTimerTimeout));
        blockSpawnTimer.Start();
    }

    private void OnTimerTimeout()
    {
        SpawnBlock(GetRandomPositionInBlockSpawningArea(), GameManager.BlockType.Standard);
    }

    public Vector2 GetRandomPositionInBlockSpawningArea()
    {
        var area = GetNode<Area2D>("SpawnArea");
        var scale = area.Scale;

        var randomXPos = _rng.RandfRange(area.Position.X, area.Position.X + scale.X);
        
        return new Vector2(randomXPos, area.Position.Y);
    }
    
    public void SpawnBlock(Vector2 position, GameManager.BlockType blockType)
    {
        PackedScene squareBlockScene = GD.Load<PackedScene>("res://scenes/square_block.tscn");
        SquareBlock instantiatedBlock = squareBlockScene.Instantiate<SquareBlock>();
        instantiatedBlock.Position = position;
        instantiatedBlock.BlockType = blockType;
        
        AddChild(instantiatedBlock, true);
    }
}