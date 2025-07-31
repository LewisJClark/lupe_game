using Godot;
using System.Collections.Generic;

public class Main : Node2D
{
   public static Main Instance;
   
   [Export] private PackedScene _levelSelectScene;
   [Export] private PackedScene[] _levels;
   
   public List<LevelData> LevelData = new List<LevelData>();

   private Level _currentLevel;

   public override void _Ready()
   {
      Instance = this;
      
      for (int i = 0; i < _levels.Length; i++)
         LevelData.Add(new LevelData());
      LevelData[0].Unlocked = true;
   }
   
   public void StartLevel(int index)
   {
      var level = _levels[index].Instance<Level>();
      level.Completed += () => OnLevelCompleted(index);
      AddChild(level);

      _currentLevel = level;
   }

   private void OnLevelCompleted(int index)
   {
      LevelData[index].Completed = true;
      if (index + 1 < LevelData.Count)
         LevelData[index + 1].Unlocked = true;
      _currentLevel.QueueFree();      
      
      AddChild(_levelSelectScene.Instance<LevelSelect>());
   }

}
