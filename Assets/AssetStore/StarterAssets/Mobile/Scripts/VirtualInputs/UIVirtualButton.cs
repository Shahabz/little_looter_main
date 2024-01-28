using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class UIVirtualButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    [System.Serializable]
    public class Event : UnityEvent { }

    [SerializeField] private Image _bg = default;
    [SerializeField] private Color _colorActive = default;
    [SerializeField] private bool _canActivate = false;
    [SerializeField] private bool _onlyPressUsage = false;
    [SerializeField] private bool _onlyReleaseUsage = false;
    [SerializeField] private bool _onPressingUsing = false;

    [Header("Output")]
    public BoolEvent buttonStateOutputEvent;
    public Event buttonClickOutputEvent;

    private bool _isActive = false;
    private bool _isPressing = false;

    public bool StatusOn { get; private set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isPressing = true;

        if (_onPressingUsing)
		{
            StartCoroutine(CheckPressingStatus());
		}

        if (_onlyReleaseUsage) return;

        if (_canActivate) return;

        OutputButtonStateValue(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPressing = false;

        if (_onPressingUsing)
        {
            //Debug.LogError("<color=red>STOP</color>");
            CancelInvoke(nameof(UpdateStatus));
            OutputButtonStateValue(false);
        }

        if (_onlyPressUsage) return;

        if (_onlyReleaseUsage)
		{
            OutputButtonStateValue(true);
        }

        OutputButtonStateValue(false);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        OutputButtonClickEvent();
    }

    public void Deactivate()
	{
        _isActive = false;

        _bg.color = Color.white;
    }

    void OutputButtonStateValue(bool buttonState)
    {
        StatusOn = buttonState;

        var state = buttonState;

        if (_canActivate)
		{
            RefreshStatus();

            state = _isActive;
		}

        buttonStateOutputEvent.Invoke(state);
    }

    void OutputButtonClickEvent()
    {
        if (_canActivate) return;

        buttonClickOutputEvent.Invoke();
    }

    private void RefreshStatus()
	{
        if (!_canActivate) return;

        _isActive = !_isActive;

        _bg.color = (_isActive) ? _colorActive : Color.white;
	}

    private IEnumerator CheckPressingStatus()
	{
        yield return new WaitForEndOfFrame();

        if (_isPressing)
        {
            //Debug.LogError("<color=green>START</color>");
            InvokeRepeating(nameof(UpdateStatus), 0, 0.5f);
        }
    }

    private void UpdateStatus()
	{
        if (_isPressing)
        {
            OutputButtonStateValue(true);
            return;
        }

        OutputButtonStateValue(false);
    }

	private void OnDisable()
	{
        CancelInvoke(nameof(UpdateStatus));

        OutputButtonStateValue(false);
    }
}
