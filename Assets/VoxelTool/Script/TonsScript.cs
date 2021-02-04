using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TonsScript : MonoBehaviour
{
    // 坂とか複数配置されてほしくないので正方形だけ対応するスタイル
    [SerializeField]
    private GameObject tonsQuad = null;     // 位置判定に使うやつ
    [SerializeField]
    private GameObject fade = null;         // 位置表示に使うやつ
    [SerializeField]
    private cubeHolder holder = null;

    private bool flagSelect = false;        // 左クリックしたか否か

    [SerializeField]
    private GameObject tonsCube = null;      // 複数配置表示用

    //private List<GameObject> putsObject = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 状態遷移用２　boolとかを消す
    /// </summary>
    public void Reset()
    {
        flagSelect = false;
        tonsCube.SetActive(false);
        tonsQuad.SetActive(false);
    }

    /// <summary>
    /// 状態遷移のやつ
    /// </summary>
    public void Tons()
    {
        if(!flagSelect)
        {
            SetTonsQuad();
        }
        else
        {
            PutTons();
        }

    }

    /// <summary>
    /// 場所決めるやつ
    /// </summary>
    void SetTonsQuad()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            fade.SetActive(true);
            fade.transform.position = holder.GetTargetPos(hit);

            if (Input.GetMouseButtonUp(0))
            {
                // この辺でQuadの表示位置設定
                tonsQuad.SetActive(true);
                tonsQuad.transform.position = fade.transform.position + new Vector3(0, -0.5f, 0);
                fade.SetActive(false);
                tonsCube.SetActive(true);
                tonsCube.transform.position = fade.transform.position;
                flagSelect = true;
            }
        }
        else
        {
            fade.SetActive(false);
        }
    }

    /// <summary>
    /// 大きさ決めるやつ
    /// </summary>
    void PutTons()
    {
        //int layerMask = LayerMask.GetMask("Tons");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100.0f, 1 << 10))     // 10番レイヤー(Tons)とだけ衝突するらしい
        {
            Vector3 pos = holder.GetTargetPos(hit);

            // tonsCubeの位置と大きさを設定
            tonsCube.transform.localScale = new Vector3(
                Mathf.Abs(pos.x - fade.transform.position.x) + 1,
                1,
                Mathf.Abs(pos.z - fade.transform.position.z) + 1);
            tonsCube.transform.position = new Vector3(
                (pos.x + fade.transform.position.x) / 2,
                pos.y,
                (pos.z + fade.transform.position.z) / 2);


            if (Input.GetMouseButtonUp(0))
            {
                // いっぱい作ってholderに入れる
                float minX, minZ, maxX, maxZ;
                minX = pos.x;
                minZ = pos.z;
                maxX = fade.transform.position.x;
                maxZ = fade.transform.position.z;
                CheckSwapToNumSize(ref maxX, ref minX);
                CheckSwapToNumSize(ref maxZ, ref minZ);
                for (int i = (int)minX; i <= (int)maxX; i++) 
                {
                    for (int j = (int)minZ; j <= (int)maxZ; j++)
                    {
                        GameObject obj = Instantiate(holder.objList[0], new Vector3(i, pos.y, j), Quaternion.identity);
                        obj.GetComponentInChildren<Renderer>().material = holder.materialList[0][holder.matDropdown.value];
                        if (holder.CompareCubeList(obj, false) == false)
                        {
                            holder.AddCubeList(obj);
                        }
                    }

                }

                Reset();
            }
            else if(Input.GetMouseButtonDown(1))
            {
                // なかったことにする
                Reset();
            }
        }
    }

    /// <summary>
    /// 大小比較しておかしかったら入れ変える
    /// refでアドレス渡し的なやつ
    /// </summary>
    /// <param name="big">大きいと思われる方</param>
    /// <param name="small">小さいと思われる方</param>
    void CheckSwapToNumSize(ref float big, ref float small)
    {
        if (big < small)
        {
            var w = big;
            big = small;
            small = w;
        }
    }

}
