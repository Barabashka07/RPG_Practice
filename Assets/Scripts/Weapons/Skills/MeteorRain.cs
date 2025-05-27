using Controllers.Entities.HealthController.Interfaces;
using UnityEngine;
using Weapons.Base;
using Weapons.Colliding;
using Weapons.ScriptableObjects;

namespace Weapons.Skills
{
    public class MeteorRain : Skill
    {
        private readonly Transform _caster;
        private Transform _castPoint;
        private GameObject _prefab;
        private SpellSkillData _spellSkillData;

        public MeteorRain(SkillData skillData,Transform castPoint, Transform caster, GameObject sword, IDamageable damageable) : base(skillData)
        {
            _caster = caster;
            _spellSkillData = (SpellSkillData)skillData;
            _prefab = _spellSkillData._projectilePrefab;
        }

        public override void Cast()
        {
            base.Cast();
            _castPoint = GameObject.FindGameObjectWithTag("Player").transform;
            var spell = Object.Instantiate(_prefab, _castPoint.position, Quaternion.identity).GetComponent<AOESpell>();
            spell.Init(_spellSkillData.damage, _caster.GetComponent<IDamageable>());
            spell.StartCoroutine(spell.Ttl());

        }
    }
}