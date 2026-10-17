using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text FN;
    public TMP_Text LN;
    public TMP_Text dates;

    public void UpdateTabInfo(Person person)
    {
        FN.text = person.FirstName;
        LN.text = person.LastName;
        dates.text = $"{person.BirthDate}-{person.DeathDate}";
    }

}
