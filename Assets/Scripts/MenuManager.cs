//Sabrina Jackson
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private IntVariable numberObjects;
    //Attach this script to a Dropdown GameObject
    Dropdown m_Dropdown;
    //This is the index value of the Dropdown
    int m_DropdownValue;

    void Start()
    {
        //Fetch the DropDown component from the GameObject
        m_Dropdown = GetComponent<Dropdown>();
        //Output the first Dropdown index value
        Debug.Log("Starting Dropdown Value : " + m_Dropdown.value);
    }

    void Update()
    {
        //Keep the current index of the Dropdown in a variable
        m_DropdownValue = m_Dropdown.value;
        switch (m_DropdownValue)
        {
            case 0: numberObjects.Value = 20;
                break;
            case 1: numberObjects.Value = 30;
                break;
            case 2: numberObjects.Value = 100;
                break;
        }
       
    }
}
