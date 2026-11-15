using UnityEngine;

public interface IStatusEffect
{
    string EffectName { get; }
    int Duration { get; set; }
    int StackCount { get; set; }

    void ApplyEffect(GameObject target);
    void RemoveEffect(GameObject target);
    void OnTurnStart();
    void OnTurnEnd();
}

