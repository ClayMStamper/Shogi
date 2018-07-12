using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DCP_CD_Comment : MonoBehaviour {

    //This script stores all of the required information for each comment
    //It keeps the links when the example comment is duplicated

    //All the fields are set in the Inspector of the example comment
    public Text displayName;
    public Text comment;
    public DCP_UIMove1DImage thisMoveImage;
    public DCP_UIMove1DText displayMoveText;
    public DCP_UIMove1DText commentMoveText;
    public DCP_UIMoveGroup fadeInGroup;

}
