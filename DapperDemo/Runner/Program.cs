using DataLayers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Runner
{
    class Program
    {
        private static IConfigurationRoot config;
        static void Main(string[] args)
        {
            Initialize();
            //Get_All_6_results();
            //var entityId =  Add_Entity();
            // Find_By_ID(entityId);
            // //Update_Entity(entityId);
            //Delete_Entity(entityId);
            //GetFullContact(entityId);
            // Get_Contact_With_IN_Clause();
            //Bulk_Insert_Contact();
            //GetStateAddress();
            GetAllContactWithAddress();
            Console.ReadLine();
        }

        private static void Initialize()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            config = builder.Build();
        }
        private static IContactRepository CreateRepository() 
        {
            var DapperString = "Data Source=.;Initial Catalog=DapperDb;Integrated Security=True";
           //Repo using Inline Query for CRUD
           // return new ContactRepository(DapperString);

            //Repo using Dapper Contrib package for CRUD
           // return new ContactRepositoryContrib(DapperString);

            //Repo uadding Stored Procedure support for CRUD
            return new ContactRepositorySP(DapperString);
        }

        private static ContactRepositoryEx GetContactRepositoryEx()
        {
            var DapperString = "Data Source=.;Initial Catalog=DapperDb;Integrated Security=True";
            return new ContactRepositoryEx(DapperString);
        }

        static void Get_Contact_With_IN_Clause()
        {
            var repo = GetContactRepositoryEx();
            var result = repo.GetContactById(1, 2, 3);
            var dynamicResult = repo.GetContactByIdDynamic(1, 2, 3);
            result.Output();
            dynamicResult.Output();

        }
        static void Get_All_6_results()
        {
            var repo = CreateRepository();

            var contact = repo.GetAll();
            Console.WriteLine($"Count: {contact.Count}");
            //Debug.Assert(contact.Count == 6);
            contact.Output();
            
        }       

        static  int  Add_Entity()
        {
            var repo = CreateRepository();
            var contact = new Contact()
            {
                FirstName = "John",
                LastName = "Doe",
                Company = "Microsoft",
                Title = "Developer",
                Email = "johndoe@gmail.com"
            };
            var address = new Address()
            {
                AddressType = "Home",
                City = "Oklahoma",
                PostalCode= "411252",
                StateId= 6,
                StreetAddress = "Bishops Road-123"
            };
            contact.Addresses = new System.Collections.Generic.List<Address>();
            contact.Addresses.Add(address);
            repo.Save(contact);
           // repo.Add(contact);

            Debug.Assert(contact.Id != 0);
            Console.WriteLine("Contact Inserted");
            Console.WriteLine($"Contact Id for newly inserted Contact is {contact.Id}");
            return contact.Id;
            //contact.Output();

        }

        static void Find_By_ID(int id)
        {
            var repo = CreateRepository();
            var result =  repo.Find(id);
            Console.WriteLine($"Entity Found with Id {result.Id}");
            result.Output();
        }

        static void Update_Entity(int id)
        {
            var repo = CreateRepository();
            var result = repo.Find(id);
            result.FirstName = "Jane";
            result.LastName = "Doeoe";
            repo.Save(result);
            //repo.Update(result);


            var repo_new = CreateRepository();
            var updated_entity  = repo_new.Find(id);
            Console.WriteLine("Modified Entity");
            updated_entity.Output();
        }

        static void GetFullContact(int id)
        {
            var repo = CreateRepository();
            var result = repo.GetFullContact(id);
            Console.WriteLine($"Entity Found with Id {result.Id}");
            result.Output();
        }
        static void Delete_Entity(int id)
        {
            var repo = CreateRepository();
            repo.Remove(id);

            var removedEntity = repo.Find(id);
            Console.WriteLine("Entity is " + (removedEntity == null? "Deleted": "Not Deleted"));      

        }
        static void Bulk_Insert_Contact()
        {
            var repo = GetContactRepositoryEx();
            var contacts = new List<Contact>()
            {
                new Contact(){ FirstName = "Gail", LastName ="Bertrem"},
                new Contact(){ FirstName = "Patrick", LastName ="Jane"},
                new Contact(){ FirstName = "Teresa", LastName ="Lisbon"},
                new Contact(){ FirstName = "Kimbal", LastName ="Cho"}

            };
            var rowsAffected = repo.BulkInsertContact(contacts);
            Console.WriteLine($"Total Rows Inserted: {rowsAffected}");            
        }

        static void GetStateAddress()
        {
            var repo = GetContactRepositoryEx();
            var result = repo.GetAddressesByState(17);
            result.Output();
        }

        static void GetAllContactWithAddress()
        {
            var repo = GetContactRepositoryEx();
            var result = repo.GetAllContactWithAddress();
            result.Output();

        }
    }
}
