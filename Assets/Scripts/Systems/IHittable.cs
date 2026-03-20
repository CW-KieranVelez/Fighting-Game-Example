using FightTest.Data;

namespace FightTest.Systems
{
    public interface IHittable
    {
        void ReceiveHit(AttackData data);
        void ReceiveThrow(AttackData data);
    }
}
