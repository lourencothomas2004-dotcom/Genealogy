using UnityEngine;
using SQLite;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class DataSet_Manager : MonoBehaviour
{
    public SQLiteConnection db;
    public string DataSet = "TreeTest.db";

    public List<Person> matchingPeople;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string dbPath = System.IO.Path.Combine(Application.persistentDataPath, DataSet);
        db = new SQLiteConnection(dbPath);

        // Make sure the tables exist
        db.CreateTable<Person>();
        db.CreateTable<NuclearFamily>();
    }

    void SavePerson(Person person)
    {
        if (person.Id == 0)
        {
            db.Insert(person);
        }
        else
        {
            db.Update(person);
        }
    }

    public void SaveFromUI(UiExtractor ui)
    {
        Person person = ui.ExtractPersonFromUI();
        person.BirthFamilyID = FindOrCreateNuclearFamily(ui.parent1ID, ui.parent2ID);

        SavePerson(person);
    }

    public void findOnName(string searchText)
    {
        matchingPeople = new List<Person>();

        string[] nameParts = searchText.Split(' ');

        if (string.IsNullOrWhiteSpace(searchText) || nameParts.Length == 0)
        {
            return;
        }
        else if (nameParts.Length == 1)
        {
            string searchWord1 = nameParts[0];
            matchingPeople = db.Table<Person>().Where(p => p.FirstName.Contains(searchWord1) || p.LastName.Contains(searchWord1)).ToList();
        }
        else if (nameParts.Length > 1)
        {
            string searchWord1 = nameParts[0];
            string searchWord2 = nameParts[1];
            matchingPeople = db.Table<Person>().Where(p => (p.FirstName.Contains(searchWord1) && p.LastName.Contains(searchWord2)) || (p.FirstName.Contains(searchWord2) && p.LastName.Contains(searchWord1))).ToList();
        }
    }

    public void UpdateDropDown(TMP_Dropdown dropdown)
    {
        dropdown.ClearOptions();

        List<string> displayNames = new List<string>();
        displayNames.Add("--- Select a person ---");

        foreach (Person person in matchingPeople)
        {
            string text = $"{person.FirstName} {person.LastName} {person.BirthDate}-{person.DeathDate}";
            displayNames.Add(text);
        }

        dropdown.AddOptions(displayNames);
    }

    public int? FindOrCreateNuclearFamily(int? pID1, int? pID2)
    {
        if (pID1 == null && pID2 == null)
        {
            return null; 
        }

        int? nuclearFamilyId;
        NuclearFamily family;
        if (pID1 is null)
        {
            family = db.Table<NuclearFamily>().Where(p => p.parent1ID == pID2).FirstOrDefault();
            if (family == null)
            {
                NuclearFamily newFamily = new NuclearFamily { parent1ID = pID2, parent2ID = null };
                db.Insert(newFamily);
                nuclearFamilyId = newFamily.Id;
            }
            else
            {
                nuclearFamilyId = family.Id;
            }
        }
        else if (pID2 is null)
        {
            family = db.Table<NuclearFamily>().Where(p => p.parent1ID == pID1).FirstOrDefault();
            if (family == null)
            {
                NuclearFamily newFamily = new NuclearFamily { parent1ID = pID1, parent2ID = null };
                db.Insert(newFamily);
                nuclearFamilyId = newFamily.Id;
            }
            else
            {
                nuclearFamilyId = family.Id;
            }
        }
        else
        {
            family = db.Table<NuclearFamily>().Where(p => (p.parent1ID == pID1 && p.parent2ID == pID2) || (p.parent1ID == pID2 && p.parent2ID == pID1)).FirstOrDefault();
            if (family == null)
            {
                NuclearFamily newFamily = new NuclearFamily { parent1ID = pID1, parent2ID = pID2 };
                db.Insert(newFamily);
                nuclearFamilyId = newFamily.Id;
            }
            else
            {
                nuclearFamilyId = family.Id;
            }
        }

        return nuclearFamilyId;
    }
    List<Person> FindAllChildren(Person parent)
    {
        List<Person> children = new List<Person>();
        List<NuclearFamily> families = db.Table<NuclearFamily>().Where(p => p.parent1ID == parent.Id || p.parent2ID == parent.Id).Distinct().ToList();

        foreach (NuclearFamily fam in families)
        {
            int id = fam.Id;
            children.AddRange(db.Table<Person>().Where(p => p.BirthFamilyID == id).ToList());
        }
        return children;
    }

    List<Person> FindChildren(Person parent1, Person parent2)
    {
        List<Person> children = new List<Person>();
        NuclearFamily family = db.Table<NuclearFamily>().Where(p => (p.parent1ID == parent1.Id && p.parent2ID == parent2.Id) || (p.parent1ID == parent2.Id && p.parent2ID == parent1.Id)).FirstOrDefault();

        if (family != null)
        {
            int familyID = family.Id;
            children.AddRange(db.Table<Person>().Where(p => p.BirthFamilyID == familyID).ToList());
        }
        return children;
    }

    void OnApplicationQuit()
    {
        if (db != null)
        {
            db.Close(); // Safely shut the vault door
        }
    }
}
