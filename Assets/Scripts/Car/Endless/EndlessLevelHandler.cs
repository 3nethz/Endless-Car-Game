using System.Collections;
using UnityEngine;

public class EndlessLevelHandler : MonoBehaviour
{
    [SerializeField]
    GameObject[] sectionPrefabs;

    GameObject[] sectionPool = new GameObject[20];

    GameObject[] sections = new GameObject[10];

    Transform playerCarTransform;

    WaitForSeconds waitFor100ms = new WaitForSeconds(0.1f);

    const float sectionLength = 26;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCarTransform = GameObject.FindGameObjectWithTag("Player").transform;

        int preFabIndex = 0;

        //Create a pool for our endless sections
        for (int i = 0; i < sectionPool.Length; i++)
        {
            sectionPool[i] = Instantiate(sectionPrefabs[preFabIndex]);
            sectionPool[i].SetActive(false);

            preFabIndex++;

            //Loop if we run out of prefabs
            if (preFabIndex > sectionPrefabs.Length - 1)
                preFabIndex = 0;
        }

        //Add the first section to the road
        for (int i = 0; i < sections.Length; i++)
        {
            //Get a random section from the pool
            GameObject randomSection = GetRandomSectionFromPool();

            //Move the section into position and set it to active
            randomSection.transform.position = new Vector3(sectionPool[i].transform.position.x, 0, i * sectionLength);
            randomSection.SetActive(true);

            //Set the section in array
            sections[i] = randomSection;
        }

        StartCoroutine(UpdateLessOftenCO());
    }


    IEnumerator UpdateLessOftenCO()
    {
        while (true)
        {
            updateSectionsPositions();
            yield return waitFor100ms;
        }

    }

    void updateSectionsPositions()
    {
        for (int i = 0; i < sections.Length; i++)
        {
            //Check if section is too far behind
            if (sections[i].transform.position.z - playerCarTransform.position.z < -sectionLength)
            {
                //Store the lastactiveposition of the section and disable it
                Vector3 lastSectionPosition = sections[i].transform.position;
                sections[i].SetActive(false);

                //Get a new random section from the sectionpool
                sections[i] = GetRandomSectionFromPool();

                //Move the new section into place and activate it
                sections[i].transform.position = new Vector3(lastSectionPosition.x, 0, lastSectionPosition.z + sectionLength * sections.Length);
                sections[i].SetActive(true);
            }
        }
    }
    GameObject GetRandomSectionFromPool()
    {
        //Pick a random index
        int randomIndex = Random.Range(0, sectionPool.Length);

        bool isNewSectionFound = false;

        while (!isNewSectionFound)
        {
            //Check if a section is not active, in that case we have found a section to load
            if (!sectionPool[randomIndex].activeInHierarchy)
                isNewSectionFound = true;
            else
            {
                randomIndex++;
                if (randomIndex > sectionPool.Length - 1)
                    randomIndex = 0;
            }
        }
        return sectionPool[randomIndex];
    }

}
