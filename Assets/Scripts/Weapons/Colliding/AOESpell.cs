using System;
using System.Collections;
using Controllers.Entities.HealthController.Interfaces;
using UnityEngine;
using Weapons.Base;

namespace Weapons.Colliding
{
    public class AOESpell : MonoBehaviour, IDamaging
    {
        private Damage _damage;
        private IDamageable _self;
        private IDamageable _damageable;

        public void Init(Damage damage, IDamageable self)
        {
            _damage = damage;
            _self = self;
        }
        public void DoDamage(IDamageable damageable)
        {
            damageable.TakeDamage(_damage);
        }

        private void OnTriggerEnter(Collider other)
        {
            _damageable = other.gameObject.GetComponent<IDamageable>();
            if (_damageable == null) return;
            if (_damageable == _self) return;
            
            
            
            InvokeRepeating(nameof(DamageRepeating), 1f, 1f);

        }
        
        private void DamageRepeating() => DoDamage(_damageable);

        private void OnTriggerExit(Collider other)
        {
            _damageable = other.gameObject.GetComponent<IDamageable>();
            if (_damageable == null) return;
            if (_damageable == _self) return;
            
            CancelInvoke(nameof(DamageRepeating));
        }
        
        public IEnumerator Ttl()
        {
            yield return new WaitForSeconds(5f);
            Destroy(gameObject);
        }
    }
}