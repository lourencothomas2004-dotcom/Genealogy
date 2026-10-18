using UnityEngine;
using UnityEngine.UI;
using TMPro;using System.Linq;
using System.Collections.Generic;

public class InfoExtractor : MonoBehaviour
{
    public DataSet_Manager Ds;
    public GameObject EditingPanel;
    public Person personInfo;

    [Header("UI References")]
    public TMP_Text FN;
    public TMP_Text LN;
    public TMP_Text BN;
    public TMP_Text DOB;
    public TMP_Text DOD;
    public TMP_Text Father;
    public TMP_Text Mother;
    public TMP_Text Siblings;
    public TMP_Text Children;
    public TMP_Text Bio;

    public void LoadPersonInfo(Person person)
    {
        personInfo = person;
        FN.text = person.FirstName;
        LN.text = person.LastName;
        BN.text = person.BirthName;
        DOB.text = person.BirthDate;
        DOD.text = person.DeathDate;
        Bio.text = person.Biography;

        if (person.BirthFamilyID is not null)
        {
                NuclearFamily family = Ds.db.Find<NuclearFamily>(person.BirthFamilyID);

                if (family is not null)
                {
                    if (family.parent1ID is not null)
                    {
                        Person mother = Ds.db.Find<Person>(family.parent1ID);
                        Mother.text = $"{mother.FirstName} {mother.LastName}";
                    }
    
                    if (family.parent2ID is not null)
                    {
                        Person father = Ds.db.Find<Person>(family.parent2ID);
                        Father.text = $"{father.FirstName} {father.LastName}";
                    }
    
                    List<Person> siblings = Ds.FindSiblings(person);
                    var siblingNames = siblings.Select(p => p.FirstName + " " + p.LastName);
    
                    if (siblings.Count == 0)
                    {
                        Siblings.text = "None";
                    }
                    else
                    {
                        string finalSiblingText = string.Join(", ", siblingNames);
                        Siblings.text = finalSiblingText;
                    }
                }
        }
    }

    public void EditPerson()
    {
        EditingPanel.GetComponent<UiExtractor>().LoadPerson(personInfo);
    }

    public void CleanPanel()
    {
        FN.text = "";
        LN.text = "";
        BN.text = "";
        DOB.text = "";
        DOD.text = "";
        Bio.text = "";
        Father.text ="";
        Mother.text = "";
        Siblings.text = "";
        Children.text = "";

        personInfo = null;
    }
}
