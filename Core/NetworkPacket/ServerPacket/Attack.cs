﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Module.CharacterData;

namespace Core.NetworkPacket.ServerPacket
{
    public class Attack : Network.ServerPacket
    {
        private readonly int _attackerObjId;
        private readonly int _targetId;
        public bool Soulshot { get; set; }
        private readonly int _grade;
        private readonly int _x;
        private readonly int _y;
        private readonly int _z;
        private List<Hit> _hits;
        
        public Attack(Character attacker, Character target, bool ss, int grade)
        {
            _attackerObjId = attacker.ObjectId;
            Soulshot = ss;
            _grade = grade;
            _x = attacker.GetX();
            _y = attacker.GetY();
            _z = attacker.GetZ();
            _hits = new List<Hit>(0);
        }

        public Attack(Character attacker, Character target)
        {
            _attackerObjId = attacker.ObjectId;
            _targetId = target.ObjectId;
            _x = attacker.GetX();
            _y = attacker.GetY();
            _z = attacker.GetZ();
        }
        
        public void AddHit(int targetId, int damage, bool miss, bool crit, bool shld)
        {
            // Get the last position in the hits table
            int pos = _hits.Count;
            Hit[] tmp = new Hit[pos + 1];

            for (int i = 0; i < _hits.Count; i++)
                tmp[i] = _hits[i];

            tmp[pos] = new Hit(targetId, damage, miss, crit, shld, Soulshot, _grade);
            _hits = new List<Hit>(tmp);
        }

        public Hit LastHit()
        {
            return _hits.FirstOrDefault();
        }
        
        public bool HasHits()
        {
            return _hits.Count > 0;
        }
        
        public override async Task WriteAsync()
        {
            await WriteByteAsync(0x05);
		
            await WriteIntAsync(_attackerObjId);
            await WriteIntAsync(_hits[0].TargetId);
            await WriteIntAsync(_hits[0].Damage);
            await WriteByteAsync(_hits[0].Flags);
            await WriteIntAsync(_x);
            await WriteIntAsync(_y);
            await WriteIntAsync(_z);
            await WriteShortAsync((short)_hits.Count - 1);
            for (int i = 1; i < _hits.Count; i++)
            {
                await WriteIntAsync(_hits[i].TargetId);
                await WriteIntAsync(_hits[i].Damage);
                await WriteByteAsync(_hits[i].Flags);
            }
        }
    }
}