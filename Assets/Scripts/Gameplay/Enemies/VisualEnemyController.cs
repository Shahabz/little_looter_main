/*
 * Date: September 16th, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
	public class VisualEnemyController : MonoBehaviour
	{
		#region Inspector

		[SerializeField] private AudioClip[] _footstepAudioClips = default;
		[Range(0, 1)] 
		[SerializeField] private float _footstepAudioVolume = 0.5f;

		#endregion

		#region Private properties

		private EnemyBehaviorData _data = default;
		private Animator _animator = default;
		private int _animSpeedId = default;
		private int _animMotionSpeedId = default;
		private int _animAttackId = default;
		private int _animIsAttackingId = default;
		private int _animTakeDamageId = default;
		private int _animDeathId = default;
		private bool _isEnabled = false;

		#endregion

		#region Public methods

		public void Init(EnemyState state, EnemyBehaviorData data)
		{
			_data = data;

			_animator = GetComponent<Animator>();

			// Locomotion ids
			_animSpeedId = Animator.StringToHash("Speed");
			_animMotionSpeedId = Animator.StringToHash("MotionSpeed");

			// Attack ids
			_animAttackId = Animator.StringToHash("attack");
			_animIsAttackingId = Animator.StringToHash("isAttacking");

			// Take damage id
			_animTakeDamageId = Animator.StringToHash("takeDamage");

			// Death id
			_animDeathId = Animator.StringToHash("dead");

			_isEnabled = true;

			Refresh(state, false);
		}

		public void Refresh(EnemyState state, bool isPerformingAttack)
		{
			if (!_isEnabled) return;

			if (state == EnemyState.IDLE)
			{
				SetIdle();
				return;
			}

			if (state == EnemyState.CHASE)
			{
				SetChase();
				return;
			}

			if (state == EnemyState.ATTACK)
			{
				SetAttack(isPerformingAttack);
				return;
			}

			if (state == EnemyState.DIE)
			{
				SetDeath();
				return;
			}
		}

		/// <summary>
		/// Invoked from animation event
		/// </summary>
		/// <param name="animationEvent"></param>
		public void OnFootstep(AnimationEvent animationEvent)
		{
			if (animationEvent.animatorClipInfo.weight <= 0.5f) return;

			if (_footstepAudioClips.Length == 0) return;

			var index = Random.Range(0, _footstepAudioClips.Length);

			AudioSource.PlayClipAtPoint(_footstepAudioClips[index], transform.TransformPoint(transform.position), _footstepAudioVolume);
		}

		public void TakeDamage()
		{
			if (!_isEnabled) return;

			SetTakeDamage();
		}

		public void Attack()
		{
			_animator.SetTrigger(_animAttackId);
		}

		#endregion

		#region Private methods

		private void SetIdle()
		{
			_animator.SetFloat(_animSpeedId, 0);
			_animator.SetFloat(_animMotionSpeedId, 0);

			_animator.SetBool(_animIsAttackingId, false);
		}

		private void SetChase()
		{
			_animator.SetFloat(_animSpeedId, _data.WalkingSpeed);
			_animator.SetFloat(_animMotionSpeedId, 1);

			_animator.SetBool(_animIsAttackingId, false);
		}

		private void SetAttack(bool isPerformingAttack)
		{
			// Stop movement animation
			_animator.SetFloat(_animSpeedId, 0);
			_animator.SetFloat(_animMotionSpeedId, 0);

			// Set attack trigger
			/*if (!isPerformingAttack)
			{
				_animator.SetTrigger(_animAttackId);
			}*/

			_animator.SetBool(_animIsAttackingId, true);
		}

		private void SetDeath()
		{
			// Stop movement animation
			_animator.SetFloat(_animSpeedId, 0);
			_animator.SetFloat(_animMotionSpeedId, 0);

			_animator.SetBool(_animIsAttackingId, false);

			// Set dead trigger
			_animator.SetTrigger(_animDeathId);
		}

		private void SetTakeDamage()
		{
			// Set take damage trigger
			_animator.SetTrigger(_animTakeDamageId);
		}

		#endregion
	}
}