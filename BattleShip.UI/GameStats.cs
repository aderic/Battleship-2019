using BattleShip.BLL.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip.UI
{
    class Stats
    {
        public uint ShotsHit { get; private set; }
        public uint ShotsMissed { get; private set; }
        public bool IsVictorious { get; private set; }

        public decimal Accuracy
        {
            get
            {
                return decimal.Divide(ShotsHit, ShotsHit + ShotsMissed);
            }
        }

        public void Update(ShotStatus status)
        {
            switch (status)
            {
                case ShotStatus.Victory:
                    IsVictorious = true;
                    ShotsHit++;
                    break;
                case ShotStatus.Hit:
                case ShotStatus.HitAndSunk:
                    ShotsHit++;
                    break;
                case ShotStatus.Miss:
                    ShotsMissed++;
                    break;
            }
        }
    }
}
