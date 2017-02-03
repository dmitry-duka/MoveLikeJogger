using System;
using System.Runtime.Serialization;

namespace MoveLikeJogger.DataContracts.Moves
{
    [DataContract]
    public class MoveStatisticsDTO
    {
        [DataMember]
        public int DaysTotal { get; set; }

        [DataMember]
        public int DaysActive { get; set; }

        [DataMember]
        public long TotalDistance { get; set; }

        [DataMember]
        public int MaxDistance { get; set; }

        [DataMember]
        public int AvgDistance { get; set; }

        [DataMember]
        public long TotalDuration { get; set; }

        [DataMember]
        public int MaxDuration { get; set; }

        [DataMember]
        public int AvgDuration { get; set; }

        [DataMember]
        public float MaxSpeed { get; set; }

        [DataMember]
        public float AvgSpeed { get; set; }

        [DataMember]
        public DateTime MaxDistanceDate { get; set; }

        [DataMember]
        public DateTime MaxDurationDate { get; set; }

        [DataMember]
        public DateTime MaxSpeedDate { get; set; }
    }
}
