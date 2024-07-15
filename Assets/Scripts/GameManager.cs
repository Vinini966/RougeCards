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
    public ActionDatabaseManager ActionManager;

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
        ActionManager = FindObjectOfType<ActionDatabaseManager>();

        Factory.HeroToMake = "BASIC_BILLY";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
