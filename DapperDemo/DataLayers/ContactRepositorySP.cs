using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;

namespace DataLayers
{
    /// <summary>
    /// CRUD using Stored Procedure Support using Dapper
    /// </summary>
    public class ContactRepositorySP : IContactRepository
    {
        private IDbConnection db;

        public ContactRepositorySP(string connection)
        {
            this.db = new SqlConnection(connection);
        }
        public Contact Add(Contact contact)
        {
            throw new NotImplementedException();
        }

        public Contact Find(int id)
        {
            return this.db.Query<Contact>("GetContact", new { Id = id }, commandType: CommandType.StoredProcedure).SingleOrDefault();
        }

        public List<Contact> GetAll()
        {
            throw new NotImplementedException();
        }

        public Contact GetFullContact(int id)
        {
            using (var multipleResults = this.db.QueryMultiple("GetContact", new { Id = id }, commandType: CommandType.StoredProcedure))
            {
                var contact = multipleResults.Read<Contact>().SingleOrDefault();
                var address = multipleResults.Read<Address>().ToList();
                if (contact != null && address.Any())
                {
                    contact.Addresses = new List<Address>();
                    contact.Addresses.AddRange(address);
                }
                return contact;
            }
        }

        public void Remove(int id)
        {
             this.db.Execute("DelteContact", new { Id = id }, commandType: CommandType.StoredProcedure);
        }

        public void Save(Contact contact)
        {
            using var trnScope = new TransactionScope();
            var paramters = new DynamicParameters();
            paramters.Add("@Id", value: contact.Id, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
            paramters.Add("@FirstName", contact.FirstName);
            paramters.Add("@LastName", contact.LastName);
            paramters.Add("@Company",contact.Company);
            paramters.Add("@Title", contact.Title);
            paramters.Add("@Email", contact.Email);
            this.db.Execute("SaveContact", paramters, commandType: CommandType.StoredProcedure);
            contact.Id = paramters.Get<int>("@Id");

            foreach (var address in contact.Addresses)
            {
                address.ContactId = contact.Id;
                var addrParam = new DynamicParameters(new {
                    ContactId = address.ContactId,
                    AddressType = address.AddressType,
                    StreetAddress = address.StreetAddress,
                    City = address.City,
                    PostalCode = address.PostalCode,
                    StateId = address.StateId
                });
                addrParam.Add("@Id", address.Id, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                this.db.Execute("SaveAddress", addrParam, commandType: CommandType.StoredProcedure);
                address.Id = addrParam.Get<int>("@Id");
                foreach (var addr in contact.Addresses)
                {
                    if(addr.IsDeleted)
                    this.db.Execute("DeleteAddress", new { Id = addr.Id },commandType: CommandType.StoredProcedure);
                }
                trnScope.Complete();
            }
        }

        public Contact Update(Contact contact)
        {
            throw new NotImplementedException();
        }
    }
}
