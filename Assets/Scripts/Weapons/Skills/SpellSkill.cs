using Controllers.Entities.HealthController.Interfaces;
using UnityEngine;
using Weapons.Base;
using Weapons.Colliding;
using Weapons.ScriptableObjects;

namespace Weapons.Skills
{
    public class SpellSkill : Skill
    {
        private readonly GameObject _spellProjectile;
        private readonly Transform _castPoint;
        private readonly Transform _caster;


        public SpellSkill(SkillData skillData,Transform castPoint, Transform caster, GameObject sword, IDamageable damageable) : base(skillData)
        {
            var _skillData = (SpellSkillData)skillData;
            _spellProjectile = _skillData._projectilePrefab;
            _castPoint = castPoint;
            _caster = caster;
        }

        public override void Cast()
        {
            base.Cast();
            var obj = Object.Instantiate(_spellProjectile,_castPoint.position, Quaternion.identity);
            var proj = obj.AddComponent<Projectile>();
            proj.Init(Damage, _castPoint.GetComponentInParent<IDamageable>());
            var rb = obj.GetComponent<Rigidbody>();
            int layerMask = ~LayerMask.GetMask(_castPoint.gameObject.tag);
            Physics.Raycast(_castPoint.position, _caster.forward, out RaycastHit hit, 1000f,layerMask);
            Vector3 castDir =  hit.point - _castPoint.position;
            rb.AddForce(castDir.normalized * 1000f);
            proj.StartCoroutine(proj.Ttl());
        }

        public void CastPlug()
        {
            base.Cast();
        }
    }
}