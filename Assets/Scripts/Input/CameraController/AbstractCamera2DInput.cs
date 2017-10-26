using UnityEngine;

public abstract class AbstractCamera2DInput : MonoBehaviour
{

	public Camera2DController TheCamera;

	public bool ClickEnabled = true;
	public bool DoubleClickEnabled = false;
	public float DoubleClickCatchTime = 0.3f;
//	public bool LongClickEnabled = false; // not implemented
	public bool ZoomEnabled = false;
	public float ZoomSpeed = 1f;
	public bool DragScreenEnabled = false;
	public float DragTreshold = 10f;

	protected bool DragStarted = false;
	protected bool ZoomStarted = false;

	protected bool IsInteractionStatic = true;

	protected Vector2 LastZoomCenter = Vector2.zero;

	// Подписка на события ввода и старт/остановка контроллера
	protected bool Attached = false;

	protected virtual void Attach()
	{
		Detach ();
		if (TheCamera == null)
		{
			TheCamera = GetComponent<Camera2DController>();
		}
		Attached = true;
	}

	protected virtual void Detach()
	{
		Attached = false;
	}

	// Подписка для внешних слушателей
	public delegate void OnClickTap(Vector2 clickPosition);
	public virtual event OnClickTap onClickTap = delegate {};
	public delegate void OnDoubleClickTap(Vector2 clickPosition);
	public virtual event OnDoubleClickTap onDoubleClickTap = delegate {};
	public delegate void OnLongClickTap(Vector2 clickPosition);
	public virtual event OnLongClickTap onLongClickTap = delegate {};

	protected void RaiseDoubleClickTap(Vector2 position)
	{
		onDoubleClickTap(position);
	}

    protected void RaiseClickTap(Vector2 position)
    {
        onClickTap(position);
    }
	
	protected void RaiseLongClickTap(Vector2 position)
	{
		onLongClickTap(position);
	}
}
