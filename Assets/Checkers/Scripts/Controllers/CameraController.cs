using Checkers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

public class CameraController : MonoBehaviour
{
	[Header("Main components:")]
	public Transform Target;
	public Image RotateFieldImage;

	[Header("Parameters:")]
	public float Speed = 1;
	public float SpeedToStop = 1;

	[Header("Serialized fields:")]
	public GameController GameController;
	public Transform LeftCanvasSide;
	public Transform RightCanvasSide;
	public EventTrigger CameraDrag;

	[Header("Camera transforms:")]
	public Transform PerspectiveTransform;
	public Transform TopDownTransform;
	public Transform StandardTransform;

  

    [Space(10f)]
	private bool _isPerspectiveCamera;
    public bool IsPerspectiveCamera
    {
        get
        {
            return _isPerspectiveCamera;
        }
        set
        {
            _isPerspectiveCamera = value;
            RotateFieldImage.raycastTarget = _isPerspectiveCamera;
			if (_isPerspectiveCamera)
			{
				PerspectiverCameraEvent?.Invoke();
			}
			else
			{
				NonPerspectiveCameraEvent?.Invoke();
			}
        }
    }

    private int _multiplier = 1;
    private float _valueToRotate;
	private float _currentFieldOfView;

    public UnityEvent PerspectiverCameraEvent;
    public UnityEvent NonPerspectiveCameraEvent;

    private void Start()
    {
        InitCameraFieldOfViewByDistance();
        InitCamera();
		InitEventTrigger();

    }   

	/// <summary>
	/// Setup field of view based on landscape width.
	/// </summary>
    private void InitCameraFieldOfViewByDistance()
    {
        var distance = Vector3.Distance(LeftCanvasSide.position,RightCanvasSide.position);
        _currentFieldOfView = 56f + Mathf.Abs(56f - ( distance * 56f) / 189.05f);
        Camera.main.fieldOfView = _currentFieldOfView;
    }
    

    /// <summary>
    /// Setup perspective camera.
    /// </summary>
    public void InitCamera()
    {
        _isPerspectiveCamera = true;
    }

	/// <summary>
	/// Init event trigger for freeze excluding.
	/// </summary>
	public void InitEventTrigger()
	{
		CameraDrag.OnDrag(new PointerEventData(EventSystem.current));
	}

	private void Update()
    {
        Rotate();
    }

	/// <summary>
	/// Rotate action.
	/// </summary>
    private void Rotate()
    {
        if (!_isPerspectiveCamera) return;

        _valueToRotate = Mathf.Lerp(_valueToRotate, 0, Time.deltaTime * SpeedToStop);
        Target.Rotate(0, 0, _valueToRotate);
    }

	/// <summary>
	/// Called when user drag camera.
	/// </summary>
    public void OnMouseDrag(BaseEventData data)
    {
        _multiplier = ((data as PointerEventData).position.y > Screen.height / 2) ? -1 : 1;
        _valueToRotate = _multiplier * (data as PointerEventData).delta.x * Speed;
    }

	/// <summary>
	/// Reset camera animation.
	/// </summary>
    public void ResetCamera()
    {
        _valueToRotate = 0f;
        Target.DOKill();
        Target.DOLocalMove(StandardTransform.transform.position, 0.2f).SetEase(Ease.OutSine);
        Target.DORotate(PerspectiveTransform.transform.rotation.eulerAngles, 0.2f, RotateMode.FastBeyond360).SetEase(Ease.OutSine);
    }

	/// <summary>
	/// Change camera state and rotation.
	/// </summary>
    public void SetCameraState()
    {
        _valueToRotate = 0f;

        IsPerspectiveCamera = !IsPerspectiveCamera;
        if (IsPerspectiveCamera)
        {
            Target.DOKill();
            Target.DOLocalMove(PerspectiveTransform.transform.position, 0.5f).SetEase(Ease.OutSine);
            Target.DORotate(PerspectiveTransform.transform.rotation.eulerAngles, 0.5f, RotateMode.FastBeyond360).SetEase(Ease.OutSine);
            DOTween.To(() => Camera.main.fieldOfView, x => Camera.main.fieldOfView = x, _currentFieldOfView, 1f);
        }
        else
        {
            Target.DOKill();
            Target.DOLocalMove(TopDownTransform.transform.position, 0.5f).SetEase(Ease.OutSine);
            Target.DORotate(TopDownTransform.transform.rotation.eulerAngles, 0.5f, RotateMode.FastBeyond360).SetEase(Ease.OutSine);
            DOTween.To(() => Camera.main.fieldOfView, x => Camera.main.fieldOfView = x, (Camera.main.aspect > 1.3 && Camera.main.aspect < 1.4) ? 65f : 56f, 1f);
        }

        if (GameController.IsGameStart)
        {
            GameController.BoardViewCompoennt.InitCurrentTurnObjects();
        }
    }
}
