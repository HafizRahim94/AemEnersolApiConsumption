using System;
using System.Collections.Generic;
using System.Text;

namespace AemEnersolProject
{
    class Platform
    {
        public int id { get; set; }
        public string uniqueName { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public List<Well> well { get; set; }

        public class Well
        {
            public int id { get; set; }
            public int platformId { get; set; }
            public string uniqueName { get; set; }
            public float latitude { get; set; }
            public float longitude { get; set; }
            public DateTime createdAt { get; set; }
            public DateTime updatedAt { get; set; }
        }
    }
}
