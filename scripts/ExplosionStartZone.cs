using Godot;
using System;
using System.Threading.Tasks;

public partial class ExplosionStartZone : Area2D
{
	[Export] public CollisionShape2D spawnArea;
	[Export] public float ExplosionWidth = 32;
	[Export] public float ExplosionHeight = 32;
	[Export] public float VerticalWaitTime = 1;
	[Export] public PackedScene ExplosionScene = GD.Load<PackedScene>("res://scenes/explosion.tscn");

	public float CurrentExplosionHeight = float.MaxValue;

	private Timer _rowSpawnTimer = new Timer();

	public override void _Ready()
	{
		_rowSpawnTimer.WaitTime = VerticalWaitTime;
		AddChild(_rowSpawnTimer);
	}

	public async void StartExplosionsAsync()
	{
		var area = spawnArea.Shape.GetRect();
		float explosionXStart = spawnArea.GlobalPosition.X;
		float explosionXEnd = explosionXStart + area.Size.X;
		float explosionYStart = spawnArea.GlobalPosition.Y;
		float explosionYEnd = explosionYStart - area.Size.Y;

		for (float explosionY = explosionYStart; explosionY > explosionYEnd; explosionY -= ExplosionHeight)
		{
			_rowSpawnTimer.Start();
			await ToSignal(_rowSpawnTimer, "timeout");
			CurrentExplosionHeight = explosionY;
			GD.Print("Current explosion height: ", CurrentExplosionHeight);
			await SpawnExplosionRow(explosionY, explosionXStart, explosionXEnd);
		}
	}

	private async Task SpawnExplosionRow(float explosionY, float start, float end)
	{
		for (float x = start; x <= end; x += ExplosionWidth)
		{
			var explosionSpawnPosition = new Vector2(x, explosionY);
			SpawnExplosion(explosionSpawnPosition);
		}
	}

	private void SpawnExplosion(Vector2 position)
	{
		var explosion = ExplosionScene.Instantiate<Node2D>();
		explosion.Position = position;
		AddChild(explosion, true);
	}
}
