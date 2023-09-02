using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

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

    [Header("Output")]
    public BoolEvent buttonStateOutputEvent;
    public Event buttonClickOutputEvent;

    private bool _isActive = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_canActivate) return;

        OutputButtonStateValue(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_onlyPressUsage) return;

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

}
