using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiExtractor : MonoBehaviour
{
    public DataSet_Manager Ds;

    [Header("UI References")]
    public TMP_InputField FN;
    public TMP_InputField LN;
    public TMP_InputField BN;
    public TMP_InputField DOB;
    public TMP_InputField DOD;
    public TMP_InputField Bio;

    public TMP_InputField Father;
    public TMP_InputField Mother;

    public int? currentPersonId;
    public int? parent1ID = null;
    public int? parent2ID = null;
    public Person ExtractPersonFromUI()
    {
        int ID = currentPersonId ?? 0;
        currentPersonId = null;
        return new Person 
        {
            Id = ID,
            FirstName = string.IsNullOrWhiteSpace(FN.text) ? null : FN.text,
            LastName = string.IsNullOrWhiteSpace(LN.text) ? null : LN.text, 
            BirthName = string.IsNullOrWhiteSpace(BN.text) ? null : BN.text, 
            BirthDate = string.IsNullOrWhiteSpace(DOB.text) ? null : DOB.text,
            DeathDate = string.IsNullOrWhiteSpace(DOD.text) ? null : DOD.text,
            Biography = string.IsNullOrWhiteSpace(Bio.text) ? null : Bio.text,
        };
    }

    public void LoadPerson(Person person)
    {
        parent1ID = null;
        parent2ID = null;
        currentPersonId = person.Id;

        FN.text = person.FirstName;
        LN.text = person.LastName;
        BN.text = person.BirthName;
        DOB.text = person.BirthDate;
        DOD.text = person.DeathDate;
        Bio.text = person.Biography;

        if (person.BirthFamilyID is not null)
        {
            int? familyId = person.BirthFamilyID;
            NuclearFamily family = Ds.db.Find<NuclearFamily>(familyId);

            if (family.parent1ID is not null)
            {
                parent1ID = family.parent1ID;
            }
            if (family.parent2ID is not null)
            {
                parent2ID = family.parent2ID;
            }
        }
    }
    public void SetParent1(int id)
    {
        parent1ID = Ds.matchingPeople[id - 1].Id;
    }
    public void SetParent2(int id)
    {
        parent2ID = Ds.matchingPeople[id - 1].Id;
    }

    public void DeletePerson()
    {
        Ds.DeleteSomeone(Ds.db.Find<Person>(currentPersonId));
        currentPersonId = null;
    }

    public void CleanPanel()
    {
        FN.text = "";
        LN.text = "";
        BN.text = "";
        DOB.text = "";
        DOD.text = "";
        Bio.text = "";
        Father.text = "";
        Mother.text = "";
        currentPersonId = null;
        parent1ID = null;
        parent2ID = null;
    }
}
