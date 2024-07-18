using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject HeroFactory;
    public Vector3 SpawnPosistion;

    public HeroFactory Factory;
    public InputManger InputManger;
    public ActionDatabaseManager ActionDatabaseManager;
    public ActionEquipManager ActionEquipManager;

    // Start is called before the first frame update
    void Awake()
    {

        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        Factory = Instantiate(HeroFactory, SpawnPosistion, Quaternion.identity, transform).GetComponent<HeroFactory>();
        InputManger = FindObjectOfType<InputManger>();
        ActionDatabaseManager = FindObjectOfType<ActionDatabaseManager>();
        ActionEquipManager = FindObjectOfType<ActionEquipManager>();

        Factory.HeroToMake = "BASIC_BILLY";

        ActionEquipManager.EquipAction(0, "BASIC_SWIPE");
        ActionEquipManager.EquipAction(1, "UP_SLASH");
        ActionEquipManager.EquipAction(2, "GROUNDPOUND");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
