using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinaryBackground : MonoBehaviour
{

    [SerializeField] private GameObject BinaryTemplate;
    [SerializeField] private Material binaryMat; //For changing colours if needed.

    const int totalColumns = 32; //How many columns of binary numbers matrix we are having?
    const int perColumn = 32; //How many binary bits per column?

    const float binaryHeight = 3.0f; //How tall is the binary mesh in world space?
    const float distance = 48.0f; //How far should these meshes be away from the player?

    class Binary
    {

        public float lifeTime = 0.0f;
        public int column = -1;
        public int row = -1;
        public List<MeshRenderer> meshes = new List<MeshRenderer>();

        public void onUpdate(float delta)
        {
            lifeTime -= delta;
            if(lifeTime <= 0.0f)
            {
                meshes[0].enabled = false;
                meshes[1].enabled = false;
            }
        }

        public void beginShow()
        {
            meshes[Random.Range(0,1)].enabled = true;
            lifeTime = 0.5f;
        }
    }

    List<List<Binary>> binarys = new List<List<Binary>>();

    float fireBinary = 0.0f;
    float maxTick = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        //Begin instantiating all those floating binary numbers around the player.
        for (int i = 0; i < totalColumns; i++)
        {
            List<Binary> column = new List<Binary>();
            float angle = (360.0f / (float)totalColumns) * (float)i * Mathf.Deg2Rad;
            for (int c = 0; c < perColumn; c++)
            {
                GameObject binary = Instantiate(BinaryTemplate);
                binary.transform.SetParent(transform);

                binary.transform.localPosition = new Vector3(
                    Mathf.Sin(angle) * distance,
                    binaryHeight * ((float)perColumn * 0.75f) - (binaryHeight * (float)c),
                    Mathf.Cos(angle) * distance
                );

                binary.transform.localRotation = Quaternion.Euler(0.0f, angle * Mathf.Rad2Deg, 0.0f);

                Binary binaryObj = new Binary();
                binaryObj.column = i;
                binaryObj.row = c;
                binaryObj.meshes.Add(binary.transform.GetChild(0).GetComponent<MeshRenderer>());
                binaryObj.meshes.Add(binary.transform.GetChild(1).GetComponent<MeshRenderer>());

                column.Add(binaryObj);
            }

            binarys.Add(column);

        }
    }

    // Update is called once per frame
    void Update()
    {
        fireBinary += Time.deltaTime;
        if(fireBinary > maxTick)
        {
            fireBinary = 0.0f;

            int column = Random.Range(0, totalColumns - 1);

            if(binarys[column][0].lifeTime <= 0.0f)
                binarys[column][0].beginShow();
        }

        for (int c = 0; c < totalColumns; c++)
            for (int r = 0; r < perColumn; r++)
                binarys[c][r].onUpdate(Time.deltaTime);

    }
}
