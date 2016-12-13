using System;

namespace LibreR.MongoDb {
    public class CollectionName {
        public Type Type { get; set; }
        public string Name { get; set; }

        public CollectionName(Type type, string name) {
            Type = type;
            Name = name;
        }
    }
}
