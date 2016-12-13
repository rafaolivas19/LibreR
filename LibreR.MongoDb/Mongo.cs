using System;
using System.Collections.Generic;
using System.Linq;
using LibreR.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace LibreR.MongoDb {
    public class Mongo {
        private readonly MongoDatabase _connection;
        private readonly Dictionary<Type, string> _collections = new Dictionary<Type, string>();

        public Mongo(string dbName, params CollectionName[] collections) : this(dbName, "27017", "127.0.0.1", collections) { }

        public Mongo(string dbName, string server, params CollectionName[] collections) : this(dbName, "27017", server, collections) { }

        public Mongo(string dbName, string port, string server, params CollectionName[] collections) {
            foreach (var c in collections) {
                var id = c.Type.GetProperty("Id");
                if (id == null) throw new LibrerException(string.Format("Object {0} doesn't contain an Id property", c.Type));
                if (id.PropertyType != typeof(ObjectId)) throw new LibrerException(string.Format("Object {0} Id property is not of type ObjectId", c.Type));

                _collections[c.Type] = c.Name;
            }

            var client = new MongoClient(string.Format("mongodb://{0}:{1}", server, port));
            _connection = client.GetServer().GetDatabase(dbName);
        }

        public void Create<T>(T element) {
            _connection.GetCollection<T>(_collections[typeof(T)]).Insert(element);
        }

        public T[] Read<T>(Func<T, bool> query) {
            return _connection.GetCollection<T>(_collections[typeof(T)]).AsQueryable<T>().Where(query).ToArray();
        }

        public T[] Read<T>(ObjectId id) {
            return Read<T>(x => id == GetId(x));
        }

        public T[] ReadAll<T>() {
            return _connection.GetCollection<T>(_collections[typeof(T)]).AsQueryable<T>().ToArray();
        }

        public void Update<T>(T element) {
            _connection.GetCollection<T>(_collections[typeof(T)]).Save(element);
        }

        public void Update<T>(T element, Action<T> action) {
            action(element);

            _connection.GetCollection<T>(_collections[typeof(T)]).Save(element);
        }

        public void Delete<T>(ObjectId id) {
            _connection.GetCollection<T>(_collections[typeof(T)]).FindAndRemove(new FindAndRemoveArgs { Query = Query.EQ("_id", id) });
        }

        public void Delete<T>(T element) {
            _connection.GetCollection<T>(_collections[typeof(T)]).FindAndRemove(new FindAndRemoveArgs { Query = Query.EQ("_id", GetId(element)) });
        }

        public void Delete<T>(Func<T, bool> query) {
            foreach (var x in _connection.GetCollection<T>(_collections[typeof(T)]).AsQueryable<T>().Where(query)) {
                _connection.GetCollection<T>(_collections[typeof(T)]).Remove(Query.EQ("_id", GetId(x)));
            }
        }

        public int Count<T>(Func<T, bool> query) {
            return _connection.GetCollection<T>(_collections[typeof(T)]).AsQueryable<T>().Where(query).Count();
        }

        public int CountAll<T>() {
            return _connection.GetCollection<T>(_collections[typeof(T)]).AsQueryable<T>().Count();
        }

        public bool Exist<T>(Func<T, bool> query) {
            return Count(query) > 0;
        }

        private static ObjectId GetId(object element) {
            return (ObjectId)element.GetType().GetProperty("Id").GetValue(element);
        }
    }
}
