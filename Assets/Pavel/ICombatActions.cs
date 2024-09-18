using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public interface ICombatActions
{
	IEnumerator Attack();
}

public class ActionUiIndicator : MonoBehaviour
{
	public void UpdateVisualProgress(float progress)
	{
    }
}

public abstract class CombatAction : MonoBehaviour, ICombatActions
{
    public abstract IEnumerator Attack();
}

public abstract class CoolodwnBasedCombatAction : CombatAction
{
	[SerializeField]
	private ActionUiIndicator _indicator;

	public override IEnumerator Attack()
	{
		yield return StartCoroutine(AttackImplementation(UpdateProgress));
	}

	private void UpdateProgress(float updateprogress)
	{
		_indicator.UpdateVisualProgress(updateprogress);
    }

	protected abstract IEnumerator AttackImplementation(Action<float> progressUpdate);
}

public abstract class TriggerBasedCombatAttack : CombatAction
{
    [SerializeField]
    private ActionUiIndicator _indicator;

    public override IEnumerator Attack()
    {
        yield return null;
        AttackImplementation();
    }

    protected abstract void AttackImplementation();
}

public class SworkAttack : CoolodwnBasedCombatAction
{
    protected override IEnumerator AttackImplementation(Action<float> progressUpdate)
    {
        // Switch weapon
        progressUpdate(0.1f);
        // Start animation
        progressUpdate(0.5f);
        // Damage
        // 
        yield return null;
    }
}

public class ShootAttack : CoolodwnBasedCombatAction
{
    protected override IEnumerator AttackImplementation(Action<float> progressUpdate)
    {
        yield return null;
    }
}

public class PulseAttach : TriggerBasedCombatAttack
{
    protected override void AttackImplementation()
    {
    }
}

public class Player : MonoBehaviour
{
    private CombatAction[] _actions;
}