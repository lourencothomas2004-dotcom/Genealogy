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

    public int? parent1ID = null;
    public int? parent2ID = null;
    public Person ExtractPersonFromUI()
    {
        return new Person 
        {
            FirstName = string.IsNullOrWhiteSpace(FN.text) ? null : FN.text,
            LastName = string.IsNullOrWhiteSpace(LN.text) ? null : LN.text, 
            BirthName = string.IsNullOrWhiteSpace(BN.text) ? null : BN.text, 
            BirthDate = string.IsNullOrWhiteSpace(DOB.text) ? null : DOB.text,
            DeathDate = string.IsNullOrWhiteSpace(DOD.text) ? null : DOD.text,
            Biography = string.IsNullOrWhiteSpace(Bio.text) ? null : Bio.text,
        };
    }

    public void SetParent1(int id)
    {
        parent1ID = Ds.matchingPeople[id - 1].Id;
    }
    public void SetParent2(int id)
    {
        parent2ID = Ds.matchingPeople[id - 1].Id;
    }
}
