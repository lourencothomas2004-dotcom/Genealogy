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

    public int minSpacex = 120;
    public int minSpacey = 200;
    public Dictionary<int, int> peopleSpace;
    public Dictionary<int, int> familySpace;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string dbPath = System.IO.Path.Combine(Application.persistentDataPath, DataSet);
        db = new SQLiteConnection(dbPath);

        // Make sure the tables exist
        db.CreateTable<Person>();
        db.CreateTable<NuclearFamily>();

        peopleSpace = new Dictionary<int, int>();
        familySpace = new Dictionary<int, int>();
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

    public int GetBloodDown(Person person)
    {
        if (peopleSpace.ContainsKey(person.Id)) 
        {
            return peopleSpace[person.Id];
        }

        int spaceNeeded = minSpacex; 

        List<NuclearFamily> families = FindAllFamilies(person);
        foreach (NuclearFamily family in families)
        {
            spaceNeeded += GetFamilySize(family); 
        }

        peopleSpace[person.Id] = spaceNeeded;
        return spaceNeeded;
    }

    public int GetFamilySize(NuclearFamily family)
    {
        if (familySpace.ContainsKey(family.Id)) 
        {
            return familySpace[family.Id];
        }

        int spaceNeeded = 0;

        if (family.parent1ID != null && family.parent2ID != null)
        {
            spaceNeeded += minSpacex;
        }

        List<Person> children = db.Table<Person>().Where(p => p.BirthFamilyID == family.Id).ToList();
        foreach (Person child in children)
        {
            spaceNeeded += GetBloodDown(child);
        }

        familySpace[family.Id] = spaceNeeded;
        return spaceNeeded;
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
            family = db.Table<NuclearFamily>().Where(p => p.parent2ID == pID2 && p.parent1ID == null).FirstOrDefault();
            if (family == null)
            {
                NuclearFamily newFamily = new NuclearFamily { parent1ID = null, parent2ID = pID2 };
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
            family = db.Table<NuclearFamily>().Where(p => p.parent1ID == pID1 && p.parent2ID == null).FirstOrDefault();
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
    
    public List<NuclearFamily> FindAllFamilies(Person parent)
    {
        List<NuclearFamily> families = db.Table<NuclearFamily>().Where(p => p.parent1ID == parent.Id || p.parent2ID == parent.Id).Distinct().ToList();

        for (int i = 0; i < families.Count; i++)
        {
            if ((families[i].parent1ID is not null && families[i].parent2ID is null) || (families[i].parent2ID is not null && families[i].parent1ID is null))
            {
                (families[0], families[i]) = (families[i], families[0]);
            }
        }

        return families;
    }
    public List<Person> FindFamilyChildren(NuclearFamily family)
    {
        List<Person> children = new List<Person>();

        if (family != null)
        {
            int familyID = family.Id;
            children.AddRange(db.Table<Person>().Where(p => p.BirthFamilyID == familyID).ToList());
        }
        return children;
    }
    public List<Person> FindAllChildren(Person parent)
    {
        List<Person> children = new List<Person>();
        List<NuclearFamily> families = FindAllFamilies(parent);

        foreach (NuclearFamily fam in families)
        {
            int id = fam.Id;
            children.AddRange(db.Table<Person>().Where(p => p.BirthFamilyID == id).ToList());
        }
        return children;
    }

    public List<Person> FindChildren(Person parent1, Person parent2)
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

    public List<Person> FindSiblings(Person person)
    {
        if (person.BirthFamilyID is not null)
        {
            return db.Table<Person>().Where(p => p.BirthFamilyID == person.BirthFamilyID && p.Id != person.Id).ToList();
        }
        return null;
    }

    public void DeleteSomeone(Person person)
    {
        int targetId = person.Id;
        List<NuclearFamily> families1 = db.Table<NuclearFamily>().Where(p => p.parent1ID == targetId).ToList();
        List<NuclearFamily> families2 = db.Table<NuclearFamily>().Where(p => p.parent2ID == targetId).ToList();

        foreach (NuclearFamily family in families1)
        {
            int currentFamilyId = family.Id;
            foreach (Person child in db.Table<Person>().Where(p => p.BirthFamilyID == currentFamilyId))
            {
                if (family.parent2ID is null)
                {
                    child.BirthFamilyID = null;
                }
                else
                {
                    child.BirthFamilyID = FindOrCreateNuclearFamily(family.parent2ID, null);
                }
                db.Update(child);
            }
            db.Delete<NuclearFamily>(family.Id);
        }

        foreach (NuclearFamily family in families2)
        {
            int currentFamilyId = family.Id;
            foreach (Person child in db.Table<Person>().Where(p => p.BirthFamilyID == currentFamilyId))
            {
                if (family.parent1ID is null)
                {
                    child.BirthFamilyID = null;
                }
                else
                {
                    child.BirthFamilyID = FindOrCreateNuclearFamily(family.parent1ID, null);
                }
                db.Update(child);
            }
            db.Delete<NuclearFamily>(family.Id);
        }
        db.Delete<Person>(person.Id);
    }
    void OnApplicationQuit()
    {
        if (db != null)
        {
            db.Close(); // Safely shut the vault door
        }
    }
}
