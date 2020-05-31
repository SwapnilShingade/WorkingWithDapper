using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DataLayers
{

    /// <summary>
    /// Repo to demonstrate support of List 
    /// in SQL Query
    /// </summary>
    public class ContactRepositoryEx
    {
        private IDbConnection db;
        public ContactRepositoryEx(string connection)
        {

            this.db = new SqlConnection(connection);
        }

        public int BulkInsertContact(List<Contact> contacts)
        {
            var sql = "INSERT INTO CONTACTS (FirstName, LastName, Email, Company, Title) VALUES (@FirstName, @LastName, @Email, @Company, @Title)" +
                "SELECT cast(scope_identity() as int)";
            return db.Execute(sql, contacts);
        }

        public List<Address> GetAddressesByState(int stateId)
        {
            return this.db.Query<Address>("Select * from Addresses where StateId ={=stateId}", new { stateId }).ToList();
        }
        public List<Contact> GetContactById(params int[] ids)
        {
            return this.db.Query<Contact>("SELECT * FROM CONTACTS WHERE ID IN @Ids", new { Ids = ids }).ToList();
        }

        public List<dynamic> GetContactByIdDynamic(params int[] ids)
        {
            return this.db.Query("SELECT * FROM CONTACTS WHERE ID IN @Ids", new { Ids = ids }).ToList();
        }
        public List<Contact> GetAllContactWithAddress()
        {
            var sql = "SELECT * FROM COntacts  AS C INNER JOIN ADDRESSES AS A ON C.ID = A.CONTACTID";
            var contactDict = new Dictionary<int, Contact>();
            var contacts = this.db.Query<Contact, Address, Contact>(sql, (contact, address) =>
            {
                if (!contactDict.TryGetValue(contact.Id, out var currentContact))
                {
                    currentContact = contact;
                    contactDict.Add(currentContact.Id, currentContact);
                }
                if (currentContact.Addresses == null)
                    currentContact.Addresses = new List<Address>();

                currentContact.Addresses.Add(address);
                return currentContact;
            });
            return contacts.Distinct().ToList();
        }
    }
}
