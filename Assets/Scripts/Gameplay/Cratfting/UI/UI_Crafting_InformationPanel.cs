/*
 * Date: December 16th, 2023
 * Author: Peche
 */

using LittleLooters.General;
using LittleLooters.Model;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_Crafting_InformationPanel : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private GameObject _content = default;
        [SerializeField] private Slider _slider = default;

        [Header("Requirement")]
        [SerializeField] private TMPro.TextMeshProUGUI _txtRequirement = default;
        [SerializeField] private Image _iconRequirement = default;
		[SerializeField] private Color _colorEnough = default;
		[SerializeField] private Color _colorNotEnough = default;
		[SerializeField] private Button _btnUp = default;
		[SerializeField] private Button _btnDown = default;
		[Header("Result")]
        [SerializeField] private TMPro.TextMeshProUGUI _txtResult = default;
        [SerializeField] private Image _iconResult = default;
		[Header("Button")]
		[SerializeField] private Button _btn = default;
		[SerializeField] private Image _btnBackground = default;
		[SerializeField] private TMPro.TextMeshProUGUI _txtButton = default;
		[SerializeField] private Sprite _spriteButtonEnabled = default;
		[SerializeField] private Sprite _spriteButtonNotEnabled = default;
		[SerializeField] private Color _colorButtonTextEnabled = default;
		[SerializeField] private Color _colorButtonTextNotEnabled = default;

		#endregion

		#region Private properties

		private int _areaId = -1;
		private int _amountSelected = 0;
		private int _playerResourceAmount = 0;
		private int _unitAmountRequired = 0;
		private int _currentAmountRequired = 0;

		#endregion

		#region Unity events

		private void OnEnable()
		{
			_btn.onClick.AddListener(Process);
			_btnUp.onClick.AddListener(IncreaseResult);
			_btnDown.onClick.AddListener(DecreaseResult);
		}

		private void OnDisable()
		{
			_btn.onClick.RemoveAllListeners();
			_btnUp.onClick.RemoveAllListeners();
			_btnDown.onClick.RemoveAllListeners();
		}

		#endregion

		#region Public methods

		public void Show(CraftingConfigurationData data, PlayerProgressData playerProgressData)
		{
            LoadInfo(data, playerProgressData);

            _content.SetActive(true);
        }

        public void Hide()
		{
            _content.SetActive(false);
		}

		#endregion

		#region Private methods

		private void LoadInfo(CraftingConfigurationData data, PlayerProgressData playerProgressData)
		{
			_areaId = data.Id;

			_amountSelected = 1;

			_iconRequirement.sprite = data.ResourceRequired.Icon;
			_txtRequirement.text = $"{data.AmountRequired}";

			_iconResult.sprite = data.ResourceGenerated.Icon;
			_txtResult.text = $"{1}";

			// Check if at least the amount of player's resources amount required is enough
			_playerResourceAmount = playerProgressData.GetResourceAmount(data.ResourceRequired.Id);
			_unitAmountRequired = data.AmountRequired;
			_currentAmountRequired = data.AmountRequired;

			RefreshSlider();

			RefreshCraftingStatus();
		}

		private void Process()
		{
			// TODO: sfx

			if (_canDebug) DebugProcess();

			if (_amountSelected < 1) return;

			UI_GameplayEvents.OnCraftingStarted?.Invoke(_areaId, _amountSelected);
		}

		/*private void OnSliderValueChanged(float value)
		{
			//if (_canDebug) DebugSliderValueChange(value);

			_amountSelected = (int)value;

			_currentAmountRequired = _amountSelected * _unitAmountRequired;

			_txtRequirement.text = $"{_currentAmountRequired}";
			_txtResult.text = $"{_amountSelected}";

			RefreshCraftingStatus();
		}*/

		private void RefreshCraftingStatus()
		{
			var isEnough = _playerResourceAmount >= _currentAmountRequired;

			// Refresh requirement amount
			_txtRequirement.color = (isEnough) ? _colorEnough : _colorNotEnough;

			// Refresh button status
			_btnBackground.sprite = (isEnough) ? _spriteButtonEnabled : _spriteButtonNotEnabled;
			_txtButton.color = (isEnough) ? _colorButtonTextEnabled : _colorButtonTextNotEnabled;
		}
		
		private void RefreshSlider()
		{
			var isEnough = _playerResourceAmount >= _currentAmountRequired;

			_slider.enabled = isEnough;

			if (!isEnough)
			{
				_slider.maxValue = 2;
				_slider.enabled = false;
				_slider.value = 1;

				return;
			}

			var maxAmount = Mathf.FloorToInt(_playerResourceAmount / _unitAmountRequired);

			_slider.maxValue = maxAmount;
			_slider.minValue = 0;
			_slider.value = 1;
		}

		private void IncreaseResult()
		{
			_amountSelected++;

			_currentAmountRequired = _amountSelected * _unitAmountRequired;

			RefreshTexts();

			RefreshButtonStatus();

			_slider.value = _amountSelected;
		}

		private void DecreaseResult()
		{
			if (_amountSelected == 1) return;

			_amountSelected--;

			_currentAmountRequired = _amountSelected * _unitAmountRequired;

			RefreshTexts();

			RefreshButtonStatus();

			_slider.value = _amountSelected;
		}

		private void RefreshButtonStatus()
		{
			var isEnough = _playerResourceAmount >= _currentAmountRequired;

			_btnBackground.sprite = (isEnough) ? _spriteButtonEnabled : _spriteButtonNotEnabled;
			_txtButton.color = (isEnough) ? _colorButtonTextEnabled : _colorButtonTextNotEnabled;
		}

		private void RefreshTexts()
		{
			_txtResult.text = $"{_amountSelected}";

			_txtRequirement.text = $"{_currentAmountRequired}";

			_txtRequirement.color = (_amountSelected > _playerResourceAmount / _unitAmountRequired) ? _colorNotEnough : _colorEnough;
		}

		#endregion

		#region Debug

		private bool _canDebug = false;

		private void DebugProcess()
		{
			Debug.LogError($"Process crafting from area <color=orange>'{_areaId}'</color>. Amount selected: <color=yellow>{_amountSelected}</color>");
		}

		private void DebugSliderValueChange(float value)
		{
			var sliderValue = Mathf.FloorToInt(value);

			Debug.LogError($"Slider value <color=orange>{sliderValue}</color>");
		}

		#endregion
	}
}