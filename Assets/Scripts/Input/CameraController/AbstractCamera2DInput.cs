using UnityEngine;

public abstract class AbstractCamera2DInput : MonoBehaviour
{

	public Camera2DController theCamera;

	public bool ClickEnabled = true;
	public bool DoubleClickEnabled = false;
	public float doubleClickCatchTime = 0.3f;
//	public bool LongClickEnabled = false; // not implemented
	public bool ZoomEnabled = false;
	public float ZoomSpeed = 1f;
	public bool DragScreenEnabled = false;
	public float dragTreshold = 10f;

	protected bool dragStarted = false;
	protected bool zoomStarted = false;

	protected bool isInteractionStatic = true;

	protected Vector2 lastZoomCenter = Vector2.zero;

	// Подписка на события ввода и старт/остановка контроллера
	protected bool attached = false;
	public virtual void Attach()
	{
		Detach ();
		if (theCamera == null)
			theCamera = GetComponent<Camera2DController>();
		attached = true;
	}
	public virtual void Detach()
	{
		attached = false;
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
