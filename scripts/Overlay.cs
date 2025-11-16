using Godot;
using System;
using GameJam49Game.scripts.globals;

public partial class Overlay : Control
{
    private GameManager _gameManager = null!;
    
    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>("/root/GameManager");
        var restartButton = GetNode<Button>("MarginContainer/RestartGameButton");
        restartButton.Pressed += _gameManager.RestartGame;
    }
}
