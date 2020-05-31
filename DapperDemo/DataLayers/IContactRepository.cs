using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayers
{
    public interface IContactRepository
    {
        Contact Find(int id);
        Contact GetFullContact(int id);
        List<Contact> GetAll();
        Contact Add(Contact contact);
        void Save(Contact contact);
        Contact Update(Contact contact);
        void Remove(int id);
    }
}
