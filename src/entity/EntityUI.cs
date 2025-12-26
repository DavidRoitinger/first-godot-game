using Godot;
using System;
using System.Threading.Tasks;

namespace FirstGodotGame;
//Todo: finish this class
public partial class EntityUI : Control
{
	public int Priority { get; set; }

	public void ShowUi(int priority)
	{
		if (priority >= Priority)
		{
			Modulate = Color.Color8(255, 255, 255);
			Priority = priority;
		}
	}
	
	public void HideUi(int priority)
	{
		if (priority >= Priority)
		{
			Modulate = Color.Color8(255, 255, 255, 0);
			Priority = 0;
		}
	}

	public async Task ShowUiForTime(int priority, int timeMs)
	{
		ShowUi(priority);
		await Task.Delay(timeMs);
		HideUi(priority);
	}

	public void _on_mouse_entered()
	{
		ShowUi(1);
	}
	
	public void _on_mouse_exited()
	{
		HideUi(1);
	}
}
