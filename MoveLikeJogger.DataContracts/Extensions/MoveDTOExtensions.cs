using System;
using System.Collections.Generic;
using System.Linq;
using MoveLikeJogger.DataContracts.Moves;
using MoveLikeJogger.DataContracts.Utils;

namespace MoveLikeJogger.DataContracts.Extensions
{
    public static class MoveDTOExtensions
    {
        public static KeyValuePair<bool, string[]> Validate(this MoveDTO dto)
        {
            var em = new List<string>();

            if (dto == null)
            {
                em.Add("Move object is null.");

                return new KeyValuePair<bool, string[]>(false, em.ToArray());
            }

            if (string.IsNullOrWhiteSpace(dto.UserId))
            {
                em.Add("User Id must be specified.");
            }

            if (dto.Date == new DateTime())
            {
                em.Add("Date must be specified.");
            }

            if (dto.Distance <= 0)
            {
                em.Add("Distance must be a positive value.");
            }

            if (dto.Duration <= 0)
            {
                em.Add("Duration must be a positive value.");
            }

            if (dto.Distance > 0 && dto.Duration > 0)
            {
                var speed = MoveUtility.CalculateSpeedKmh(dto.Distance, dto.Duration);

                if (speed > MoveUtility.HumanSpeedWorldRecord * 1.1)
                {
                    em.Add("Speed is considered over human abilities!");
                }
            }

            return new KeyValuePair<bool, string[]>(!em.Any(), em.ToArray());
        }
    }
}
