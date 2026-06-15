using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TelegromV4.Models;

namespace TelegromV4.Services;

public class ContactService
{
    private const string ContactsFile = "contacts.json";
    private List<Contact> _contacts = new List<Contact>();

    public ContactService()
    {
        LoadContacts();
    }

    private void LoadContacts()
    {
        if (File.Exists(ContactsFile))
        {
            var json = File.ReadAllText(ContactsFile);
            _contacts = JsonSerializer.Deserialize<List<Contact>>(json) ?? new List<Contact>();
        }
    }

    private void SaveContacts()
    {
        var json = JsonSerializer.Serialize(_contacts, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ContactsFile, json);
    }

    public void AddContact(string ownerNickname, string contactNickname, string customName = "")
    {
        if (!_contacts.Any(c => c.OwnerNickname == ownerNickname && c.ContactNickname == contactNickname))
        {
            _contacts.Add(new Contact
            {
                OwnerNickname = ownerNickname,
                ContactNickname = contactNickname,
                CustomName = string.IsNullOrEmpty(customName) ? contactNickname : customName
            });
            SaveContacts();
        }
    }

    public void RemoveContact(string ownerNickname, string contactNickname)
    {
        var contact = _contacts.FirstOrDefault(c => c.OwnerNickname == ownerNickname && c.ContactNickname == contactNickname);
        if (contact != null)
        {
            _contacts.Remove(contact);
            SaveContacts();
        }
    }

    public void UpdateContactName(string ownerNickname, string contactNickname, string newCustomName)
    {
        var contact = _contacts.FirstOrDefault(c => c.OwnerNickname == ownerNickname && c.ContactNickname == contactNickname);
        if (contact != null)
        {
            contact.CustomName = newCustomName;
            SaveContacts();
        }
    }

    public string GetContactDisplayName(string ownerNickname, string contactNickname)
    {
        var contact = _contacts.FirstOrDefault(c => c.OwnerNickname == ownerNickname && c.ContactNickname == contactNickname);
        return contact?.CustomName ?? contactNickname;
    }

    public List<Contact> GetUserContacts(string nickname)
    {
        return _contacts.Where(c => c.OwnerNickname == nickname).ToList();
    }

    public bool IsContact(string ownerNickname, string contactNickname)
    {
        return _contacts.Any(c => c.OwnerNickname == ownerNickname && c.ContactNickname == contactNickname);
    }
}