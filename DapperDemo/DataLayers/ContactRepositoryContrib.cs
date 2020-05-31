using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace DataLayers
{
    public class ContactRepositoryContrib : IContactRepository
    {
        private IDbConnection db;
        public ContactRepositoryContrib(string connection)
        {
            db = new SqlConnection(connection);
        }
        public Contact Add(Contact contact)
        {
            var id = db.Insert<Contact>(contact);
            contact.Id = (int)id;
            return contact;
        }

        public Contact Find(int id)
        {
            return db.Get<Contact>(id);
        }

        public List<Contact> GetAll()
        {
            return db.GetAll<Contact>().ToList();
        }

        public Contact GetFullContact(int id)
        {
            return null;
        }

        public void Remove(int id)
        {
            db.Delete(new Contact { Id = id }); 
        }

        public void Save(Contact contact)
        {
            throw new NotImplementedException();
        }

        public Contact Update(Contact contact)
        {
            db.Update(contact);
            return contact;
        }
    }
}
