using UnityEngine;

public class CardAction : Card
{
	public enum ActionType
	{
		NoTarget,
		Area
	}

	[Header("CardAction")] public ActionType Type;

	public override void Drag()
	{
		base.Drag();
	}
	
	public override bool Activate(Vector2 position)
	{
		base.Activate(position);
		return true;
	}
	
}
