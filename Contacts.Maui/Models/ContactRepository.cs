using Microsoft.Maui.Controls.PlatformConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Contacts.Maui.Models
{
    public static class ContactRepository
    {
        private const string fileName = "contactRepository";
        public static List<Contact> contacts;

        static ContactRepository()
        {
            LoadContacts();
        }

        public static List<Contact> GetContacts() => contacts;
        public static Contact GetContactById(int contactId)
        {
            var contact = contacts.FirstOrDefault(x => x.ContactId == contactId);
            if(contact != null)
            {
                return new Contact()
                {
                    ContactId = contact.ContactId,
                    Name = contact.Name,
                    Email = contact.Email,
                    Phone = contact.Phone,
                    Address = contact.Address
                };
            }

            return null;
        }

        public static void UpdateContact(int contactId, Contact contact)
        {
            if (contactId != contact.ContactId) return;

            var contactToUpdate = contacts.FirstOrDefault(x => x.ContactId == contactId);
            if (contactToUpdate != null)
            {
                contactToUpdate.Name = contact.Name;
                contactToUpdate.Email = contact.Email;
                contactToUpdate.Phone = contact.Phone;
                contactToUpdate.Address = contact.Address;
            }

            SaveContacts();
        }

        public static void AddContact(Contact contact)
        {
            int maxId = 0;
            if(contacts.Count > 0)
            {
                maxId = contacts.Max(x => x.ContactId);
            }
            contact.ContactId = maxId + 1;
            contacts.Add(contact);

            SaveContacts();
        }

        public static void DeleteContact(int contactId)
        {
            var contact = contacts.FirstOrDefault(x => x.ContactId == contactId);
            if(contact != null)
            {
                contacts.Remove(contact);
            }

            SaveContacts();
        }

        internal static List<Contact> SearchContacts(string text)
        {
            return contacts.Where(x => !string.IsNullOrWhiteSpace(x.Name) && x.Name.Contains(text, StringComparison.OrdinalIgnoreCase))?.ToList();
        }

        private static void LoadContacts()
        {
            try
            {
                var rawData = File.ReadAllText(GetFilePath());
                contacts = JsonSerializer.Deserialize<List<Contact>>(rawData);
            }
            catch 
            {
                contacts = new List<Contact>();
            }
        }

        private static void SaveContacts()
        {
            var serializedData = JsonSerializer.Serialize(contacts);
            File.WriteAllText(GetFilePath(), serializedData);
        }

        private static string GetFilePath()
        {
            string filePath = string.Empty;
            var platform = DeviceInfo.Platform;

            if(platform == DevicePlatform.Android)
            {
                filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
            }
            else if (platform == DevicePlatform.iOS)
            {
                filePath = Path.Combine(FileSystem.AppDataDirectory, "..", "Library", fileName);
            }
            else if(platform == DevicePlatform.WinUI)
            {
                filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), fileName);
            }
            else
            {
                throw new NotSupportedException("Platform not supported");
            }

            return filePath;
        }
    }
}
