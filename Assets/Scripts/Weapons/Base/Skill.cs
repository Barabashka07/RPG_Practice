using UnityEngine;
using Weapons.ScriptableObjects;

namespace Weapons.Base
{
    public abstract class Skill : ISkill
    {
        private SkillData SkillData;
        public SkillType SkillType { get; private set; }
        public Damage Damage { get; private set; }
        private float _cooldownTime;
        private float timeUntilReady { get; set; } = 0f;
        public bool _isReady { get; private set; }

        protected Skill(SkillData skillData)
        {
            Damage = skillData.damage;
            SkillData = skillData;
            SkillType = SkillData.skillType;
            _cooldownTime = SkillData.cooldownTime;
        }

        private void CheckCooldown()
        {
            if (timeUntilReady <= 0)
            {
                _isReady = true;
                timeUntilReady = 0;
            }
            else
            {
                _isReady = false;
                timeUntilReady -= Time.deltaTime;
            }
        }

        public float GetReadyPercent() => Mathf.Clamp(timeUntilReady / _cooldownTime,0f,1f);
       

        public void Tick()
        {
            CheckCooldown();
        }


        public virtual void Cast()
        {
            if(!_isReady) return;
            timeUntilReady = _cooldownTime;
            
        }
    }

    public enum SkillType
    {   
        Melee,
        Fireball,
        Punch,
        Heavy,
        Meteor
    }
}