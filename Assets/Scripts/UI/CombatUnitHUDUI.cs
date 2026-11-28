using UnityEngine;
using UnityEngine.UI;

namespace Antigravity.UI
{
    public class CombatUnitHUDUI : MonoBehaviour
    {
        [Header("Unit Info")]
        public Text UnitNameText;
        public Text UnitLevelRoleText;
        public Image UnitAvatar;

        [Header("Health & Shield")]
        public Slider HealthSlider;
        public Text HealthValueText;
        public GameObject ShieldBadge;
        public Text ShieldValueText;

        [Header("Intent & Status")]
        public GameObject IntentContainer;
        public Text IntentActionText;
        public Text IntentDamageText;
        public GameObject BossBadge;

        [Header("Target Tracking")]
        public BattleUnit TargetUnit;
        public bool IsAllied = true;

        public void BindUnit(BattleUnit unit, bool isAllied)
        {
            TargetUnit = unit;
            IsAllied = isAllied;
            RefreshDisplay();
        }

        private void Update()
        {
            if (TargetUnit != null)
            {
                UpdateDynamicStats();
            }
        }

        public void RefreshDisplay()
        {
            if (TargetUnit == null) return;

            if (UnitNameText != null)
            {
                UnitNameText.text = TargetUnit.UnitName;
            }

            if (UnitLevelRoleText != null)
            {
                string role = IsAllied ? (TargetUnit.APContribution >= 4 ? "TANK" : "DPS") : (TargetUnit.EnemyData != null && TargetUnit.EnemyData.IsBoss ? "STAGE BOSS" : "MINION");
                UnitLevelRoleText.text = role;
            }

            if (BossBadge != null)
            {
                BossBadge.SetActive(!IsAllied && TargetUnit.EnemyData != null && TargetUnit.EnemyData.IsBoss);
            }

            UpdateDynamicStats();
        }

        public void UpdateDynamicStats()
        {
            if (TargetUnit == null) return;

            int hp = TargetUnit.CurrentHP;
            int maxHp = TargetUnit.MaxHP > 0 ? TargetUnit.MaxHP : 100;
            int shield = TargetUnit.CurrentShield;

            if (HealthSlider != null)
            {
                HealthSlider.maxValue = maxHp;
                HealthSlider.value = hp;
            }

            if (HealthValueText != null)
            {
                HealthValueText.text = $"{hp:#,##0} / {maxHp:#,##0} HP";
            }

            if (ShieldBadge != null)
            {
                ShieldBadge.SetActive(shield > 0);
            }

            if (ShieldValueText != null && shield > 0)
            {
                ShieldValueText.text = $"+{shield} AEGIS";
            }

            // Cập nhật Intent nếu là phe địch
            if (!IsAllied && IntentContainer != null)
            {
                IntentContainer.SetActive(!TargetUnit.IsDead);
            }

            gameObject.SetActive(!TargetUnit.IsDead);
        }

        public void SetIntent(string actionName, int damage = 0)
        {
            if (IntentContainer != null)
            {
                IntentContainer.SetActive(true);
            }
            if (IntentActionText != null)
            {
                IntentActionText.text = actionName;
            }
            if (IntentDamageText != null)
            {
                IntentDamageText.text = damage > 0 ? $"{damage} DMG" : "";
            }
        }
    }
}
