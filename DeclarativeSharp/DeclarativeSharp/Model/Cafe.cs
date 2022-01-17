using System.Collections.Generic;

namespace DeclarativeSharp.Model {
    public class Cafe {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public List<Maid> Maids { get; set; }
    }
}
