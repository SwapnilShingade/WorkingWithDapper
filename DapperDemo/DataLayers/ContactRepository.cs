using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Transactions;

namespace DataLayers
{
    public class ContactRepository : IContactRepository
    {
        private IDbConnection db;

        public ContactRepository(string connection)
        {
            this.db = new SqlConnection(connection);
        }
        public Contact Add(Contact contact)
        {
            var sql = "Insert into CONTACTS (FirstName, LastName, Email, Company, Title) values (@FirstName, @LastName, @Email, @Company, @Title);" +
                "select cast(scope_identity() as int)";
            var id = this.db.Query<int>(sql, contact).Single();
            contact.Id = id;
            return contact;
        }

        public Address Add(Address address)
        {
            var sql = "Insert into Addresses (ContactId, AddressType, StreetAddress, City, StateId, PostalCode ) values (@ContactId, @AddressType, @StreetAddress," +
                "@City, @StateId, @PostalCode);" +
                "select cast(scope_identity() as int)";
            var id = this.db.Query<int>(sql, address).SingleOrDefault();
            address.Id = id;
            return address;
        }
        public Contact Find(int id)
        {
            return this.db.Query<Contact>("Select * from Contacts where Id = @Id", new { id }).SingleOrDefault();
        }

        public List<Contact> GetAll()
        {
            return db.Query<Contact>("SELECT * FROM CONTACTS").ToList();
        }

        public Contact GetFullContact(int id)
        {
            var sql = "Select * from Contacts where Id=@id; " +
                "Select * from Addresses where ContactId =@id";
            using (var multipleResults = db.QueryMultiple(sql, new { Id = id }))
            {
                var contact = multipleResults.Read<Contact>().SingleOrDefault();
                var addresses = multipleResults.Read<Address>().ToList();
                if (contact != null && addresses != null)
                {
                    if (contact.Addresses == null)
                    {
                        contact.Addresses = new List<Address>();
                    }
                    contact.Addresses.AddRange(addresses);
                }
                return contact;
            }
        }

        public void Remove(int id)
        {
            db.Execute("Delete from Contacts where id = @id", new { id });
        }

        public void Save(Contact contact)
        {
            using var taxScope = new TransactionScope();
            
                if (contact.IsNew)
                {
                    this.Add(contact);
                }
                else
                {
                    this.Update(contact);
                }
                foreach (var addr in contact.Addresses.Where(x => !x.IsDeleted))
                {
                    addr.ContactId = contact.Id;
                    if (addr.IsNew)
                    {
                        this.Add(addr);
                    }

                    else
                    {
                        this.Update(addr);
                    }
                }

                foreach (var addr in contact.Addresses.Where(x => x.IsDeleted))
                {
                    this.db.Execute("Delete from Addressses where Id=@Id", new { addr.Id });

                
                taxScope.Complete();

            }
            
        }

        public Contact Update(Contact contact)
        {
            var sql = "Update Contacts " +
                "set FirstName = @FirstName, " +
                "  LastName = @LastName, " +
                "  Title = @Title, " +
                "  Email= @Email, " +
                "  Company = @Company " +
                "where Id = @Id";
            this.db.Execute(sql, contact);
            return contact;

        }

        public Address Update(Address address)
        {
            var sql = "Update Addresses " +
                "set AddressType = @AddressType, " +
                " StreeAddress = @StreetAddresss, " +
                " City = 2City, " +
                " StateId = @StateId, " +
                " PostalCode = @PostalCode, " +
                "where Id= @Id";
            this.db.Execute(sql, address);
            return address;
        }
    }
}
